using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
	public class AdminController : Controller
	{
		private readonly UserManager<SiteUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public AdminController(UserManager<SiteUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<IActionResult> DelegateAdmin()
		{
			var principal = this.User;
			var user = await _userManager.GetUserAsync(principal);

			var role = new IdentityRole()
			{
				Name = "Admin"
			};
			if (!await _roleManager.RoleExistsAsync("Admin"))
			{
				await _roleManager.CreateAsync(role);
			}
			await _userManager.AddToRoleAsync(user, "Admin");
			return RedirectToAction(nameof(ManageAll));
		}

		[Authorize(Roles = "Admin")]
		public IActionResult ControlPanel()
		{
			return View(_userManager);
		}

		[Authorize(Roles = "Admin")]
		public IActionResult ManageAll()
		{
			return View(_userManager);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> RemoveAdminRights(string userId)
		{
			var user = _userManager.Users.FirstOrDefault(t => t.Id == userId);
			await _userManager.RemoveFromRoleAsync(user, "Admin");
			return RedirectToAction(nameof(ManageAll));
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GrantAdminRights(string userId)
		{
			var user = _userManager.Users.FirstOrDefault(t => t.Id == userId);
			await _userManager.AddToRoleAsync(user, "Admin");
			return RedirectToAction(nameof(ManageAll));
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteUser(string userId)
		{
			var user = _userManager.Users.FirstOrDefault(t => t.Id == userId);
			await _userManager.DeleteAsync(user);
			return RedirectToAction(nameof(ManageAll));
		}
	}
}