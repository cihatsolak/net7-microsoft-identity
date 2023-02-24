namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class RoleTestController : Controller
    {
        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.message = "Bu sayfayı görmeye yetkiniz yoktur. Yetki almak için  yöneticiniz ile görüşebilirsiniz.";
            return View();
        }
    }
}
