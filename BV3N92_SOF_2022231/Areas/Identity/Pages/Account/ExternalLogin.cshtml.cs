// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Net;
using Newtonsoft.Json;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Backend.Helpers;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;

namespace Backend.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ExternalLoginModel : PageModel
{
	private readonly SignInManager<SiteUser> _signInManager;
	private readonly UserManager<SiteUser> _userManager;
	private readonly IUserStore<SiteUser> _userStore;
	private readonly IUserEmailStore<SiteUser> _emailStore;
	private readonly IEmailSender _emailSender;
	private readonly ILogger<ExternalLoginModel> _logger;
	BlobServiceClient serviceClient;
	BlobContainerClient containerClient;
	IHubContext<UserEventsHub> _hub;

	public class TokenModel
	{
		public string access_token { get; set; }
		public string token_type { get; set; }
	}

	public class MsMetaData
	{
		[JsonProperty("@odata.mediaContentType")]
		public string odatamediaContentType { get; set; }
		public string id { get; set; }
		public int width { get; set; }
		public int height { get; set; }
	}

	public ExternalLoginModel(
		SignInManager<SiteUser> signInManager,
		UserManager<SiteUser> userManager,
		IUserStore<SiteUser> userStore,
		ILogger<ExternalLoginModel> logger,
		IEmailSender emailSender,
		IHubContext<UserEventsHub> hub)
	{
		var builder = WebApplication.CreateBuilder();

		_signInManager = signInManager;
		_userManager = userManager;
		_userStore = userStore;
		_emailStore = GetEmailStore();
		_logger = logger;
		_emailSender = emailSender;

		serviceClient = new BlobServiceClient(builder.Configuration.GetConnectionString("Blobservice"));
		containerClient = serviceClient.GetBlobContainerClient(builder.Configuration.GetConnectionString("ContainerName"));
		_hub = hub;
	}

	/// <summary>
	///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	[BindProperty]
	public InputModel Input { get; set; }

	/// <summary>
	///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	public string ProviderDisplayName { get; set; }

	/// <summary>
	///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	public string ReturnUrl { get; set; }

	/// <summary>
	///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	[TempData]
	public string ErrorMessage { get; set; }

	/// <summary>
	///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
	///     directly from your code. This API may change or be removed in future releases.
	/// </summary>
	public class InputModel
	{
		[Display(Name = "Write about yourself (Bio)")]
		[StringLength(500)]
		public string Bio { get; set; }

		[Required]
		[Display(Name = "Age")]
		[Range(18, 100)]
		public int Age { get; set; }

		[Display(Name = "Orientation")]
		[Required]
		public Orientation Orientation { get; set; }

		[Display(Name = "Hobbies")]
		[Required]
		public List<string> Hobbies { get; set; }

		[Required]
		[Display(Name = "Gender")]
		public Gender Gender { get; set; }

		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		public string ProfilePicture { get; set; }
		public ICollection<IFormFile> UserPictures { get; set; }
	}

	public IActionResult OnGet() => RedirectToPage("./Login");

	public IActionResult OnPost(string provider, string returnUrl = null)
	{
		// Request a redirect to the external login provider.
		var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
		var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
		return new ChallengeResult(provider, properties);
	}

	public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
	{
		returnUrl = returnUrl ?? Url.Content("~/");
		if (remoteError != null)
		{
			ErrorMessage = $"Error from external provider: {remoteError}";
			return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
		}
		var info = await _signInManager.GetExternalLoginInfoAsync();
		if (info == null)
		{
			ErrorMessage = "Error loading external login information.";
			return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
		}

		// Sign in the user with this external login provider if the user already has a login.
		var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
		if (result.Succeeded)
		{
			_logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
			return LocalRedirect(returnUrl);
		}
		if (result.IsLockedOut)
		{
			return RedirectToPage("./Lockout");
		}
		else
		{
			// If the user does not have an account, then ask the user to create an account.
			ReturnUrl = returnUrl;
			ProviderDisplayName = info.ProviderDisplayName;
			if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
			{
				Input = new InputModel
				{
					Email = info.Principal.FindFirstValue(ClaimTypes.Email),
					FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
				};
				if (info.ProviderDisplayName == "Facebook")
				{
					var access_token_json = new WebClient().DownloadString("https://graph.facebook.com/oauth/access_token?client_id=2794042890728401&client_secret=2f287125c74430fd2dfc8cddbabae3f7&grant_type=client_credentials");
					var token = JsonConvert.DeserializeObject<TokenModel>(access_token_json);
					Input.ProfilePicture = $"https://graph.facebook.com/{info.Principal.FindFirstValue(ClaimTypes.NameIdentifier)}/picture?type=large&access_token={token.access_token}";

				}
			}
			return Page();
		}
	}

	public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
	{
		returnUrl = returnUrl ?? Url.Content("~/");
		// Get the information about the user from the external login provider
		var info = await _signInManager.GetExternalLoginInfoAsync();
		if (info == null)
		{
			ErrorMessage = "Error loading external login information during confirmation.";
			return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
		}

		if (ModelState.IsValid)
		{
			var user = CreateUser();
			user.FirstName = Input.FirstName;
			user.Age = Input.Age;
			user.Bio = Input.Bio;
			user.Orientation = Input.Orientation;
			user.Gender = Input.Gender;

			for (int i = 0; i < Input.Hobbies.Count; i++)
			{
				if (Input.Hobbies[i] == "false")
				{
					user.Hobbies.Add(new Hobby() { Name = Constants.Hobbies[i].Name, User = user, UserId = user.Id, IsChecked = false });
				}
				else
				{
					user.Hobbies.Add(new Hobby() { Name = Input.Hobbies[i], User = user, UserId = user.Id, IsChecked = true });
				}
			}

			var client = new HttpClient();
			var response = await client.GetAsync(Input.ProfilePicture);

			if (response.IsSuccessStatusCode)
			{
				var stream = await response.Content.ReadAsStreamAsync();
				BlobClient blobClient = containerClient.GetBlobClient(user.Id + "_profilepicture_0");
				using (var uploadFileStream = stream)
				{
					await blobClient.UploadAsync(uploadFileStream, true);
				}
				blobClient.SetAccessTier(AccessTier.Cool);

				user.ProfilePictureUrl = blobClient.Uri.AbsoluteUri;
			}

			if (Input.UserPictures != null)
			{
				foreach (var photo in Input.UserPictures)
				{
					var pic = new Picture();
					BlobClient blobClient = containerClient.GetBlobClient(user.Id + "_custompic_" + pic.PictureId);
					using (var uploadFileStream = photo.OpenReadStream())
					{
						await blobClient.UploadAsync(uploadFileStream, true);
					}
					blobClient.SetAccessTier(AccessTier.Cool);
					pic.PhotoUrl = blobClient.Uri.AbsoluteUri;
					pic.User = user;
					pic.UserId = user.Id;
					user.Pictures.Add(pic);
				}
			}

			await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
			await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

			var result = await _userManager.CreateAsync(user);
			if (result.Succeeded)
			{
				result = await _userManager.AddLoginAsync(user, info);
				if (result.Succeeded)
				{
					_logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

					var userId = await _userManager.GetUserIdAsync(user);
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = userId, code = code },
						protocol: Request.Scheme);

					await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
						$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

					await _hub.Clients.All.SendAsync("userCreated");

					// If account confirmation is required, we need to show the link if we don't have a real email sender
					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("./RegisterConfirmation", new { Email = Input.Email });
					}

					await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
					return LocalRedirect(returnUrl);
				}
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		ProviderDisplayName = info.ProviderDisplayName;
		ReturnUrl = returnUrl;
		return Page();
	}

	private SiteUser CreateUser()
	{
		try
		{
			return Activator.CreateInstance<SiteUser>();
		}
		catch
		{
			throw new InvalidOperationException($"Can't create an instance of '{nameof(SiteUser)}'. " +
				$"Ensure that '{nameof(SiteUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
				$"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
		}
	}

	private IUserEmailStore<SiteUser> GetEmailStore()
	{
		if (!_userManager.SupportsUserEmail)
		{
			throw new NotSupportedException("The default UI requires a user store with email support.");
		}
		return (IUserEmailStore<SiteUser>)_userStore;
	}
}
