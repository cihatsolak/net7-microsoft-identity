namespace AspNetCoreIdentityApp.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;

        public MemberController(
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager,
            IFileProvider fileProvider)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _fileProvider = fileProvider;
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

        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeInput passwordChangeInput)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var checkOldPassword = await _userManager.CheckPasswordAsync(user, passwordChangeInput.PasswordOld);
            if (!checkOldPassword)
            {
                ModelState.AddModelError("Eski şifreniz yanlış");
                return View();
            }

            var resultChangePassword = await _userManager.ChangePasswordAsync(user, passwordChangeInput.PasswordOld, passwordChangeInput.PasswordNew);
            if (!resultChangePassword.Succeeded)
            {
                ModelState.AddModelErrorList(resultChangePassword.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(user, passwordChangeInput.PasswordNew, true, false);

            TempData["SuccessMessage"] = "Şifreniz başarıyla değiştirilmiştir";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            UserEditViewModel userEditViewModel = new()
            {
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                BirthDate = user.BirthDate,
                City = user.City,
                Gender = user.Gender,
            };

            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            user.UserName = request.UserName;
            user.Email = request.Email;
            user.PhoneNumber = request.Phone;
            user.BirthDate = request.BirthDate.Value;
            user.City = request.City;
            user.Gender = request.Gender;

            if (request.Picture != null && request.Picture.Length > 0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                string randomFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Picture.FileName)}";
                string newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "userpictures").PhysicalPath, randomFileName);

                using var stream = new FileStream(newPicturePath, FileMode.Create);
                await request.Picture.CopyToAsync(stream);

                user.Picture = randomFileName;
            }

            var updateToUserResult = await _userManager.UpdateAsync(user);
            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(user);
            await _signInManager.SignOutAsync();

            if (request.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(user, true, new[] { new Claim("birthdate", user.BirthDate.ToString()) });
            }
            else
            {
                await _signInManager.SignInAsync(user, true);
            }

            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirilmiştir";

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = user.UserName!,
                Email = user.Email!,
                Phone = user.PhoneNumber!,
                BirthDate = user.BirthDate,
                City = user.City,
                Gender = user.Gender,
            };

            return View(userEditViewModel);
        }
    }
}
