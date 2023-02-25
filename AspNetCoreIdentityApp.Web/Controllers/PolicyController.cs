namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class PolicyController : Controller
    {
        [Authorize(Policy = "CityRestriction")]
        public IActionResult CityRestriction()
        {
            return View();
        }

        [Authorize(Policy = "ExchangeRestriction")]
        public IActionResult ExchangeRestriction()
        {
            return View();
        }

        [Authorize(Policy = "ViolenceRestriction")]
        public IActionResult ViolenceRestriction()
        {
            return View();
        }
    }
}
