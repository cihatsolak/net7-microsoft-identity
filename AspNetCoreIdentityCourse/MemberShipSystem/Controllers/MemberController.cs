using Mapster;
using MemberShip.Web.Models;
using MemberShip.Web.Tools;
using MemberShip.Web.Tools.Enums;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Controllers
{
    [Authorize(Roles = Role.ADMIN + "," + Role.MANAGER + "," + Role.MEMBER)]
    public class MemberController : BaseController
    {
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
            : base(userManager, signInManager, roleManager)
        {

        }

        [HttpGet]
        public IActionResult Index()
        {
            //CurrentUser : BaseController'dan geliyor.
            var userViewModel = CurrentUser.Adapt<UserViewModel>(); //Mapster (Nuget Package.)

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

            bool isCorrect = await _userManager.CheckPasswordAsync(CurrentUser, model.OldPassword); //Kullanıcının girdiği eski şifre doğru mu?

            if (!isCorrect)
            {
                ModelState.AddModelError(nameof(model.OldPassword), ErrorMessage.PASSWORD_WRONG);
                return View(model);
            }

            IdentityResult result = await _userManager.ChangePasswordAsync(CurrentUser, model.OldPassword, model.NewPassword); //Şifre değiştirme işlemi

            if (!result.Succeeded)
            {
                AddModelErrors(result); //BaseController'a yazdığım ortak method.
                return View(model);
            }

            await _userManager.UpdateSecurityStampAsync(CurrentUser); // Kullanıcının şifresini değiştirdiğim için bunu güncellemem gerekir.

            /*
             * Veritabanındaki security stamp değerini güncellediğim için cookie'deki bilgiyi güncellemek adına, kullanıcıya hissettirmeden
             * önce çıkış sonra giriş işlemi yaptırıyorum.
             */
            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(CurrentUser, model.NewPassword, true, false);

            return View();
        }

        [HttpGet]
        public IActionResult Edit()
        {
            var userViewModel = CurrentUser.Adapt<UserViewModel>(); //CurrentUser baseController'dan geliyor.

            ViewBag.Genders = new SelectList(Enum.GetNames(typeof(Gender)));

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model, IFormFile userPicture)
        {
            if (!ModelState.IsValid)
                return View(model);

            ViewBag.Genders = new SelectList(Enum.GetNames(typeof(Gender)));

            var user = CurrentUser;

            if (userPicture != null && userPicture.Length > 0)
            {
                var pictureName = $"{Guid.NewGuid()} {Path.GetExtension(userPicture.FileName)}"; //rastgele isim. Çakışma olmaması adına.

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/pictures", pictureName); //resmin kaydedileceği yer.

                using var stream = new FileStream(path, FileMode.Create);
                await userPicture.CopyToAsync(stream);

                user.Picture = $"/img/pictures/{pictureName}";
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.CityName = model.CityName;
            user.BirthDate = model.BirthDate;
            user.Gender = (int)model.Gender;

            IdentityResult result = await _userManager.UpdateAsync(user); //Bilgileri güncelle.

            if (!result.Succeeded)
            {
                AddModelErrors(result);
                return View(model);
            }

            await _userManager.UpdateSecurityStampAsync(user); //Kullanıcı bilgilerinde değişiklik olduğu için güncelliyorum.

            /*
             * Uygulamamızın hem mobil hem de web sayfası varsa ve ben web'de kullanıcı adını değiştirip aşağıdaki işlemleri yapmazsam,
             * mobilden girecek arkadaş farklı username, web den girecek arkadaş farklı username görür. Bundan dolayı giriş çıkış yaptırarak
             * cookie de yer alan bilgileri güncellemiş oluyorum.
             */
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, true);

            return View();
        }

        [Authorize(Roles = "Editor")]
        [HttpGet]
        public IActionResult EditorPage()
        {
            return View();
        }
    }
}
