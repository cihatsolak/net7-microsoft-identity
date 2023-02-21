namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public MemberController(
            SignInManager<AppUser> signInManager, 
            UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
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

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userViewModel = new UserViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber
            };

            return View(userViewModel);
        }
    }
}
