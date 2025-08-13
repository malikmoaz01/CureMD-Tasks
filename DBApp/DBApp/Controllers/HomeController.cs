using Microsoft.AspNetCore.Mvc;

namespace DBApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Dashboard()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                return RedirectToAction("Login", "Auth");
            }
            
            return View();
        }
    }
}