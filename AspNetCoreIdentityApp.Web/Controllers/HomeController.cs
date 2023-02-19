namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            UserManager<AppUser> userManager,
            ILogger<HomeController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
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

            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View();
        }
    }
}