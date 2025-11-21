using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CURDPractice_CA.UI.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    [Route("[area]")]
    [Route("[controller]")]

    [Area("Admin")]
    public class HomeController : Controller
    {
        //[Route("admin/home/index")]
        [Route("[action]")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
