using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Backend.Helpers;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Backend.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<SiteUser> _userManager;
		private readonly SignInManager<SiteUser> _signInManager;
		BlobServiceClient serviceClient;
		BlobContainerClient containerClient;

		public HomeController(ILogger<HomeController> logger, UserManager<SiteUser> userManager, SignInManager<SiteUser> signInManager)
		{
			var builder = WebApplication.CreateBuilder();

			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			serviceClient = new BlobServiceClient(builder.Configuration.GetConnectionString("Blobservice"));
			containerClient = serviceClient.GetBlobContainerClient(builder.Configuration.GetConnectionString("ContainerName"));
		}
		public IActionResult Match()
		{
			return View(_userManager);
		}
		public IActionResult Visitor()
		{
			return View();
		}
		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Privacy()
		{
			var principal = this.User;
			var user = await _userManager.GetUserAsync(principal);
			return View();
		}

		public IActionResult Main()
		{
			return View();
		}

		[Authorize]
		public IActionResult Profile()
		{
			return View();
		}

		public IActionResult Delete()
		{
			return View();
		}

		[Authorize]
		public IActionResult Manage()
		{
			return View();
		}

		[Authorize]
		public IActionResult Chat()
		{
			return View();
		}

		[Authorize]
		public async Task<IActionResult> EditProfileAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			var editUser = new EditSiteUser();
			editUser.FirstName = user.FirstName;
			editUser.Age = user.Age;
			editUser.Orientation = user.Orientation;
			editUser.Gender = user.Gender;
			editUser.Bio = user.Bio;
			editUser.ProfilePictureUrl = user.ProfilePictureUrl;

			return View(editUser);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> EditProfileAsync(EditSiteUser editUser)
		{
			var user = await _userManager.GetUserAsync(User);

			user.FirstName = editUser.FirstName;
			user.Age = editUser.Age;
			user.Bio = editUser.Bio;
			user.Gender = editUser.Gender;
			user.Orientation = editUser.Orientation;

			if (editUser.ProfilePicture != null)
			{
				var profPic = containerClient.GetBlobs().Where(x => x.Name.Contains(user.Id + "_profilepicture")).FirstOrDefault();
				var num = profPic.Name.Last() - '0';

				BlobClient blobClient = containerClient.GetBlobClient(user.Id + "_profilepicture_" + num);
				await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

				if (num == 9)
					num = -1;

				blobClient = containerClient.GetBlobClient(user.Id + "_profilepicture_" + ++num);
				using (var uploadFileStream = editUser.ProfilePicture.OpenReadStream())
				{
					await blobClient.UploadAsync(uploadFileStream, true);
				}
				blobClient.SetAccessTier(AccessTier.Cool);

				user.ProfilePictureUrl = blobClient.Uri.AbsoluteUri;
			}

			await _userManager.UpdateAsync(user);

			return RedirectToAction(nameof(Profile));
		}

		[Authorize]
		public async Task<IActionResult> DeleteUser()
		{
			var user = await _userManager.GetUserAsync(User);

			foreach (var blob in containerClient.GetBlobs().Where(x => x.Name.Contains(user.Id)))
			{
				await containerClient.GetBlockBlobClient(blob.Name).DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);
			}

			await _signInManager.SignOutAsync();
			await _userManager.DeleteAsync(user);
			return RedirectToAction(nameof(Visitor));
		}

		public IActionResult Bonus()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		public IActionResult GetImage(string userid)
		{
			return new FileContentResult(new byte[10], "image/jpeg");
		}

		public async Task<IActionResult> LikeUser(string userId)
		{
			var user = await _userManager.GetUserAsync(User);
			var likedUser = await _userManager.FindByIdAsync(userId);

			user.LikedUsers.Add(new LikedUser { LikedBy = user, LikedById = user.Id, WhoLiked = likedUser, WhoLikedId = likedUser.Id });

			await _userManager.UpdateAsync(user);
			return RedirectToAction(nameof(Match));
		}

		public async Task<IActionResult> DislikeUser(string userId)
		{
			var user = await _userManager.GetUserAsync(User);
			var likedUser = await _userManager.FindByIdAsync(userId);

			user.DislikedUsers.Add(new DislikedUser { DislikedBy = user, DislikedById = user.Id, WhoDisliked = likedUser, WhoDislikedId = likedUser.Id });

			await _userManager.UpdateAsync(user);
			return RedirectToAction(nameof(Match));
		}
	}
}