namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class PolicyController : Controller
    {
        [Authorize(Policy = "CityRestriction")]
        public IActionResult CityRestriction()
        {
            return View();
        }
    }
}
