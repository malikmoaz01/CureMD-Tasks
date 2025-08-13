using Microsoft.AspNetCore.Mvc;
using DBApp.Models;
using DBApp.Services;
using DBApp.Repositories;

namespace DBApp.Controllers
{
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientService ;

        PatientController(IPatientRepository patientService)
        {
            _patientService = patientService;
        }


        public IActionResult Patient()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPatient(Patient model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _patientService.AuthenticateAsync(model);

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
    }
    
}