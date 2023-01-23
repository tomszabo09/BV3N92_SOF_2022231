using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
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
            return View();
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
		public async Task<IActionResult> DeleteUser()
		{
			var user = _userManager.GetUserAsync(User);

			foreach (var blob in containerClient.GetBlobs().Where(x => x.Name.Contains(user.Result.Id)))
			{
				await containerClient.GetBlockBlobClient(blob.Name).DeleteAsync();
			}

			await _signInManager.SignOutAsync();
			await _userManager.DeleteAsync(user.Result);
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
	}
}