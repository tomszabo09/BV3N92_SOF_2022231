using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Backend.Data;
using Backend.Helpers;
using Backend.Hubs;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Backend.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<SiteUser> _userManager;
		private readonly SignInManager<SiteUser> _signInManager;
		ApplicationDbContext _context;
		BlobServiceClient serviceClient;
		BlobContainerClient containerClient;
		IHubContext<UserEventsHub> _hub;

		public HomeController(ILogger<HomeController> logger, UserManager<SiteUser> userManager, SignInManager<SiteUser> signInManager, IHubContext<UserEventsHub> hub, ApplicationDbContext context)
		{
			var builder = WebApplication.CreateBuilder();

			_context = context;
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			serviceClient = new BlobServiceClient(builder.Configuration.GetConnectionString("Blobservice"));
			containerClient = serviceClient.GetBlobContainerClient(builder.Configuration.GetConnectionString("ContainerName"));
			_hub = hub;
		}

		[Authorize]
		public IActionResult Match()
		{
			return View(_userManager);
		}

		[Authorize]
		public IActionResult Matched()
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

		public IActionResult Privacy()
		{
			return View();
		}

		[Authorize]
		public IActionResult Profile()
		{
			return View();
		}

		[Authorize]
		public IActionResult Chat()
		{
			return View();
		}


		[Authorize]
		[HttpGet("{id}")]
		public async Task<IActionResult> PrivateChat(string id, string chatId)
		{
            var user = await _userManager.GetUserAsync(User);
            ChatModel chatModel = new ChatModel();
			chatModel.Name = id;
            if (chatId == null)
			{
                chatModel.Name += user.Id;
            }
			chatModel.Users.Add(user);
            chatModel.Users.Add(_userManager.Users.FirstOrDefault(t => t.Id == id));

			bool exists = true;

            if (_context.Chats.FirstOrDefault(t => t.Name == chatModel.Name) == null && _context.Chats.FirstOrDefault(t => t.Name == user.Id + id) == null)
			{
				exists = false;
                _context.Chats.Add(chatModel);
                await _context.SaveChangesAsync();
            }


			if (chatId == null && !exists)
			{
                chatModel = _context.Chats.Include(t => t.Messages).FirstOrDefault(c => c.Id == chatModel.Id);
            }
			else
			{
                chatModel = _context.Chats.Include(t => t.Messages).FirstOrDefault(c => c.Name == chatModel.Name || (c.Name.StartsWith(user.Id) && c.Name.Contains(id.Substring(0, 10))));
            }

            return View(chatModel);
		}

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMessage(string chatId, string userId, string input)
        {
            var user = await _userManager.GetUserAsync(User);
            var message = new MessageModel()
			{
				ChatId = chatId,
				Text = input,
                Name = user.FirstName + ":",
                Timestamp = DateTime.Now
			};
            var xd = _context.Messages;
            _context.Messages.Add(message);
			
			await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PrivateChat), new { id = userId, chatId });
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
			editUser.Hobbies = user.Hobbies;

			foreach (var item in editUser.Hobbies)
			{
				if (item.IsChecked)
				{
					editUser.HobbyNames.Add("true");
				}
				else
				{
					editUser.HobbyNames.Add("false");
				}
			}

			return View(editUser);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> EditProfileAsync(EditSiteUser editUser)
		{
			var user = await _userManager.GetUserAsync(User);

			bool notSameImportantDetails = false;
			if (user.FirstName != editUser.FirstName || user.ProfilePictureUrl != editUser.ProfilePictureUrl || user.Age != editUser.Age || user.Bio != editUser.Bio || user.Gender != editUser.Gender)
				notSameImportantDetails = true;

			user.FirstName = editUser.FirstName;
			user.Age = editUser.Age;
			user.Bio = editUser.Bio;
			user.Gender = editUser.Gender;
			user.Orientation = editUser.Orientation;

			foreach (Hobby current in user.Hobbies)
			{
				if (editUser.HobbyNames.SingleOrDefault(h => h == current.Name) == null && current.IsChecked)
					current.IsChecked = false;
				else if (editUser.HobbyNames.SingleOrDefault(h => h == current.Name) != null && !current.IsChecked)
					current.IsChecked = true;
			}

			BlobClient blobClient;

			if (editUser.ProfilePicture != null)
			{
				var profPic = containerClient.GetBlobs().Where(x => x.Name.Contains(user.Id + "_profilepicture")).FirstOrDefault();
				var num = profPic?.Name.Last() - '0';

				blobClient = containerClient.GetBlobClient(user.Id + "_profilepicture_" + num);
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

			if (editUser.UserPictures?.Count > 0)
			{
				foreach (var picture in editUser.UserPictures)
				{
					var pic = new Picture();
					blobClient = containerClient.GetBlobClient(user.Id + "_custompic_" + pic.PictureId);
					using (var uploadFileStream = picture.OpenReadStream())
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

			await _userManager.UpdateAsync(user);

			if (notSameImportantDetails)
				await _hub.Clients.All.SendAsync("userModified");

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

			await _hub.Clients.All.SendAsync("userDeleted");

			return RedirectToAction(nameof(Visitor));
		}

		[Authorize]
		public async Task<IActionResult> DeletePicture(string id)
		{
			var user = await _userManager.GetUserAsync(User);
			user.Pictures.Remove(user.Pictures.First(p => p.PictureId == id));
			await _userManager.UpdateAsync(user);

			var blob = containerClient.GetBlobs().FirstOrDefault(b => b.Name == user.Id + "_custompic_" + id);
			await containerClient.GetBlobClient(blob.Name).DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots);

			var editUser = new EditSiteUser();
			editUser.FirstName = user.FirstName;
			editUser.Age = user.Age;
			editUser.Orientation = user.Orientation;
			editUser.Gender = user.Gender;
			editUser.Bio = user.Bio;
			editUser.ProfilePictureUrl = user.ProfilePictureUrl;

			return View("EditProfile", editUser);
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

			LikedUser thisUserLiked = likedUser.LikedUsers.FirstOrDefault(t => t.WhoLikedId == user.Id);
			if (thisUserLiked != null)
			{
				user.MatchedUsers.Add(new MatchedUser { LikedBy = user, LikedById = user.Id, WhoLiked = likedUser, WhoLikedId = likedUser.Id });
				likedUser.MatchedUsers.Add(new MatchedUser { LikedBy = likedUser, LikedById = likedUser.Id, WhoLiked = user, WhoLikedId = user.Id });

				await _userManager.UpdateAsync(user);
				return RedirectToAction(nameof(Matched));
			}

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