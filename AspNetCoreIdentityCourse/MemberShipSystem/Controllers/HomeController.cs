using Microsoft.AspNetCore.Mvc;

namespace MemberShipSystem.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
