using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult ControlPanel()
        {
            return View();
        }

        public IActionResult ManageAll()
        {
            return View();
        }
    }
}
