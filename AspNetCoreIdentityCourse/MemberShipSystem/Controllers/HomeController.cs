using MemberShip.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MemberShipSystem.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
