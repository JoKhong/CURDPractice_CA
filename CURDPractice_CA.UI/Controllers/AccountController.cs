using CURD_Practice.Controllers;
using CURDPractice_CA.Core.Domain.IdentityEntities;
using CURDPractice_CA.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CURDPractice_CA.UI.Controllers
{
    [Route("[controller]")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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

            IdentityResult result =  await _userManager.CreateAsync(user, registerDto.Password);

            if(result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
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

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto, string? ReturnUrl)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage);
                return View(loginDto);
            }

            //TODO: Save user Data Into DB
            ApplicationUser user = new ApplicationUser()
            {
                Email = loginDto.Email
            };

            //IdentityResult result = await _userManager.CreateAsync(user, loginDto.Password);

            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {  
                if(!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                
                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }
            else
            {  
                ModelState.AddModelError("Login", "Invalid Email or Password");
                return View(loginDto);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        public async Task<IActionResult> IsEmailRegistered(string email)
        {
            ApplicationUser? user =  await _userManager.FindByEmailAsync(email);

            if (user == null) 
            {
                return Json(true); // Valid
            }

            return Json(false);

        }


    }
}
