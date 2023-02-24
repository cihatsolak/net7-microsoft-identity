namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        private readonly IEmailService _emailService;
        public HomeController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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

        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordInput forgetPasswordInput)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordInput.Email);
            if (user is null)
            {
                ModelState.AddModelError("Bu email adresine sahip kullanıcı bulunamamıştır.");
                return View();
            }

            string passwordResestToken = await _userManager.GeneratePasswordResetTokenAsync(user); //token'ın ömrü program.cs'de belirlenir.

            var passwordResetLink = Url.Action("ResetPassword", "Home", new { userId = user.Id, Token = passwordResestToken }, HttpContext.Request.Scheme);

            await _emailService.SendResetPasswordEmailAsync(passwordResetLink, user.Email);

            TempData["SuccessMessage"] = "Şifre yenileme linki, eposta adresinize gönderilmiştir";

            return RedirectToAction(nameof(ForgetPassword));
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordResetInput passwordResetInput)
        {
            var userId = TempData["userId"].ToString();
            var token = TempData["token"].ToString();

            if (userId == null || token == null)
            {
                ModelState.AddModelError("Kullanıcı bulunamamıştır.");
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ModelState.AddModelError("Kullanıcı bulunamamıştır.");
                return View();
            }
            
            var identityResult = await _userManager.ResetPasswordAsync(user, token, passwordResetInput.Password);
            if (identityResult.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir";
            }
            else
            {
                ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            }

            return View();
        }
    }
}