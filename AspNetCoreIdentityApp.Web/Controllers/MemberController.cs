namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;

        public MemberController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        //public async Task<IActionResult> Logout() //1.yol
        //{
        //    await _signInManager.SignOutAsync();
        //    return RedirectToAction("Index", "Home");
        //}

        public async Task Logout() //2.yol: Hangi sayfaya yönleneceğinin bilgisi çıkış yap butonunun (html) returnurl'inde (asp-route-returnurl) 
        {
            await _signInManager.SignOutAsync();
        }
    }
}
