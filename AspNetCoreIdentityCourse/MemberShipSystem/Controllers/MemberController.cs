using Mapster;
using MemberShip.Web.Models;
using MemberShip.Web.Tools;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MemberShip.Web.Controllers
{
    public class MemberController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            var userViewModel = user.Adapt<UserViewModel>(); //Mapster (Nuget Package.)

            return View(userViewModel);
        }

        [HttpGet]
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByNameAsync(User.Identity.Name); //Kullanıcıyı buluyorum.

            bool isCorrect = await _userManager.CheckPasswordAsync(user, model.OldPassword); //Kullanıcının girdiği eski şifre doğru mu?

            if (!isCorrect)
            {
                ModelState.AddModelError(nameof(model.OldPassword), ErrorMessage.PASSWORD_WRONG);
                return View(model);
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword); //Şifre değiştirme işlemi

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            /*
             * Kullanıcının şifresini değiştirdiğim için bunu güncellemem gerekir.
             * Bunun sonucunda 20 dakika(benim belirlediğim süre) sonra kullanıcıya otomatik olarak çıkış yaptıracak.
             */
            await _userManager.UpdateSecurityStampAsync(user); 

            return View();
        }
    }
}
