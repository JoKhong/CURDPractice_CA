using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CURD_Practice.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            IExceptionHandlerPathFeature? excpetionHandlePathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            
            if(excpetionHandlePathFeature != null && excpetionHandlePathFeature.Error != null)
            {
                ViewBag.ErrorMessage = excpetionHandlePathFeature.Error.Message;
            }
            
            return View();
        }
    }
}
