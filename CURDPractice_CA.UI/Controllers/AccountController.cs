using CURD_Practice.Controllers;
using CURDPractice_CA.Core.Domain.IdentityEntities;
using CURDPractice_CA.Core.DTO;
using CURDPractice_CA.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;



namespace CURDPractice_CA.UI.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
                //Check status of radio button
                if(registerDto.UserType == UserTypeOptions.Admin)
                {
                    //Create admin role
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                        {
                            Name = UserTypeOptions.Admin.ToString(),
                        };
                        await _roleManager.CreateAsync(applicationRole);
                    }

                    await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());

                    //Add the new user to admin role
                }
                else
                {
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                        {
                            Name = UserTypeOptions.User.ToString(),
                        };
                        await _roleManager.CreateAsync(applicationRole);
                    }

                    await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
                }

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
                ApplicationUser? foundUser = await _userManager.FindByEmailAsync(loginDto.Email);

                if (foundUser != null) 
                {
                    if(await _userManager.IsInRoleAsync(foundUser, UserTypeOptions.Admin.ToString()))
                    {
                        return RedirectToAction("Index", "Home" , new { area = "admin"});
                    }
                    
                }


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

        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [Route("[action]")]
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
