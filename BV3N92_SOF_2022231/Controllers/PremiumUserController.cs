using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
	public class PremiumUserController : Controller
	{
		private readonly UserManager<SiteUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public PremiumUserController(UserManager<SiteUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		[Authorize(Roles = "PremiumUser")]
		public IActionResult PremiumLikes()
		{
			return View(_userManager);
		}

		public async Task<IActionResult> DelegatePremiumUser()
		{
			var principal = this.User;
			var user = await _userManager.GetUserAsync(principal);

			var role = new IdentityRole()
			{
				Name = "PremiumUser"
			};

			if (!await _roleManager.RoleExistsAsync("PremiumUser"))
			{
				await _roleManager.CreateAsync(role);
			}
			await _userManager.AddToRoleAsync(user, "PremiumUser");
			return RedirectToAction(nameof(PremiumLikes));
		}
	}
}
