using AspNetCoreIdentityApp.Web.Extensions;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public HomeController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInInput signInInput, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            returnUrl ??= Url.Action("Index", "Home");

            var user = await _userManager.FindByEmailAsync(signInInput.Email);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Email veya şifre yanlış");
                return View();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, signInInput.Password, signInInput.RememberMe, true);
            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError("3 dakika boyunca giriş yapamazsınız.");
                return View();
            }

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            int failedCount = await _userManager.GetAccessFailedCountAsync(user);

            ModelState.AddModelErrorList(new List<string>()
            {
                "Email veya şifre yanlış",
                $"Başarısız giriş sayısı : {failedCount}"
            });

            return View();
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpInput signUpInput)
        {
            if (!ModelState.IsValid)
            {
                return View(signUpInput);
            }

            var user = new AppUser
            {
                UserName = signUpInput.UserName,
                PhoneNumber = signUpInput.Phone,
                Email = signUpInput.Email
            };

            var identityResult = await _userManager.CreateAsync(user, signUpInput.Password);
            if (identityResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Kayıt işlemi başarıyla gerçekleştirildi.";
                return RedirectToAction(nameof(HomeController.SignUp));
            }

            ModelState.AddModelErrorList(identityResult.Errors);

            return View();
        }
    }
}