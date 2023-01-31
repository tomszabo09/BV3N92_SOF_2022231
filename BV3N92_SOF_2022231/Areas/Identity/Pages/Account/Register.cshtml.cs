// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Backend.Helpers;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;

namespace Backend.Areas.Identity.Pages.Account
{
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<SiteUser> _signInManager;
		private readonly UserManager<SiteUser> _userManager;
		private readonly IUserStore<SiteUser> _userStore;
		private readonly IUserEmailStore<SiteUser> _emailStore;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;
		BlobServiceClient serviceClient;
		BlobContainerClient containerClient;
		IHubContext<UserEventsHub> _hub;

		public RegisterModel(
			UserManager<SiteUser> userManager,
			IUserStore<SiteUser> userStore,
			SignInManager<SiteUser> signInManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender,
			IHubContext<UserEventsHub> hub)
		{
			var builder = WebApplication.CreateBuilder();

			_userManager = userManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
			serviceClient = new BlobServiceClient(builder.Configuration.GetConnectionString("Blobservice"));
			containerClient = serviceClient.GetBlobContainerClient(builder.Configuration.GetConnectionString("ContainerName"));
			_hub = hub;
		}

		[BindProperty]
		public InputModel Input { get; set; }
		public string ReturnUrl { get; set; }
		public IList<AuthenticationScheme> ExternalLogins { get; set; }

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

			[Required]
			[Display(Name = "First Name")]
			public string FirstName { get; set; }

			[Required]
			[EmailAddress]
			[Display(Name = "Email")]
			public string Email { get; set; }

			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }

			[Required]
			public IFormFile ProfilePicture { get; set; }
			public ICollection<IFormFile> UserPictures { get; set; }
		}


		public async Task OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
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

				BlobClient blobClient = containerClient.GetBlobClient(user.Id + "_profilepicture_0");
				using (var uploadFileStream = Input.ProfilePicture.OpenReadStream())
				{
					await blobClient.UploadAsync(uploadFileStream, true);
				}
				blobClient.SetAccessTier(AccessTier.Cool);

				user.ProfilePictureUrl = blobClient.Uri.AbsoluteUri;

				if (Input.UserPictures != null)
				{
					foreach (var photo in Input.UserPictures)
					{
						var pic = new Picture();
						blobClient = containerClient.GetBlobClient(user.Id + "_custompic_" + pic.PictureId);
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
				var result = await _userManager.CreateAsync(user, Input.Password);

				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					var userId = await _userManager.GetUserIdAsync(user);
					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
						protocol: Request.Scheme);

					await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
						$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

					await _hub.Clients.All.SendAsync("userCreated", user);

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
					}
					else
					{
						await _signInManager.SignInAsync(user, isPersistent: false);
						return LocalRedirect(returnUrl);
					}
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed, redisplay form
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
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
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
}
