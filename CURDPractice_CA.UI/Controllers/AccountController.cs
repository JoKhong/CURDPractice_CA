using CURD_Practice.Controllers;
using CURDPractice_CA.Core.Domain.IdentityEntities;
using CURDPractice_CA.Core.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CURDPractice_CA.UI.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if(ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select( y => y.ErrorMessage);
                return View(registerDto);
            }

            //TODO: Save user Data Into DB
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = registerDto.Email,
                PersonName = registerDto.PersonName
            };

            IdentityResult result =  await _userManager.CreateAsync(user);

            if(result.Succeeded)
            {
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }

                return View(registerDto);
            }

        }
    }
}
