using CURD_Practice.Controllers;
using CURDPractice_CA.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CURDPractice_CA.UI.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        [Route("[action]")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Register(RegisterDto registerDto)
        {
            if(ModelState.IsValid == false)
                return View(registerDto);

            //TODO: Save user Data Into DB

            return RedirectToAction( nameof(PersonsController.Index), "Persons");
        }
    }
}
