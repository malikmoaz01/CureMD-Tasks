using Microsoft.AspNetCore.Mvc;
using DBApp.Models;
using DBApp.Services;

namespace DBApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = await _authService.AuthenticateAsync(model);
            
            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                
                return RedirectToAction("Dashboard", "Home");
            }
            
            ViewBag.Error = "Invalid email or password";
            return View(model);
        }
        
        public IActionResult Signup()
        {
            return View();
        }

        

        [HttpPost]
        public async Task<IActionResult> Signup(SignupDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var result = await _authService.RegisterAsync(model);
            
            if (result)
            {
                ViewBag.Success = "Registration successful! Please login.";
                return View("Login");
            }
            
            ViewBag.Error = "Email already exists or registration failed";
            return View(model);
        }
        
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}