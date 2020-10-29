using Mapster;
using MemberShip.Web.Models;
using MemberShip.Web.Models.ViewModels;
using MemberShip.Web.Services.TwoFactorServices;
using MemberShip.Web.Tools.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Controllers
{
    public class MemberController : BaseController
    {
        private readonly ITwoFactorService _twoFactorService;
        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, ITwoFactorService twoFactorService)
            : base(userManager, signInManager, roleManager)
        {
            _twoFactorService = twoFactorService;
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
        public async Task<IActionResult> PasswordChange(ChangePasswordViewModel model)
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

            var user = CurrentUser;

            var userPhoneNumber = await _userManager.GetPhoneNumberAsync(user);

            if (!userPhoneNumber.Equals(model.PhoneNumber)) //Telefon numarası değiştirmiş demektir.
            {
                var isRegisteredPhoneNumber = _userManager.Users.Any(p => p.PhoneNumber.Equals(model.PhoneNumber)); //Girilen telefon db'de var mı?

                if (isRegisteredPhoneNumber)
                {
                    ModelState.AddModelError(nameof(model.PhoneNumber), ErrorMessage.PHONE_NUMBER_USE);
                    return View(model);
                }
            }

            ViewBag.Genders = new SelectList(Enum.GetNames(typeof(Gender)));

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

        [Authorize(Roles = Role.EDITOR)]
        [HttpGet]
        public IActionResult EditorPage()
        {
            return View();
        }

        [Authorize(Policy = Policy.CITY)]
        [HttpGet]
        public IActionResult CityClaimPage()
        {
            return View();
        }

        [Authorize(Policy = Policy.AGE)]
        [HttpGet]
        public IActionResult AgeClaimPage()
        {
            return View();
        }

        [Authorize(Policy = Policy.EXCHANGE)]
        [HttpGet]
        public async Task<IActionResult> Exchange()
        {
            bool hasClaim = User.HasClaim(p => p.Type.Equals(ClaimName.EXPIRE_DATE_EXCHANGE));

            if (!hasClaim)
            {
                var addedDate = DateTime.Now.AddDays(30).Date.ToShortDateString(); //30 Gün boyunca erişebilsin.

                Claim expirteDateExchangeClaim = new Claim(ClaimName.EXPIRE_DATE_EXCHANGE, addedDate, ClaimValueTypes.String, ClaimName.ISSUER);

                await _userManager.AddClaimAsync(CurrentUser, expirteDateExchangeClaim); //Claim ekliyorum.

                //Eklediğim claim'in aktif olması için, kullanıcıya arka tarafta çıkış-giriş yaptırıyorum. Tarayıcı daki cookie değeri de değişsin diye.
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(CurrentUser, true);
            }

            return View();
        }

        [HttpGet]
        public IActionResult TwoFactorAuth()
        {
            var authenticationViewModel = new AuthenticationViewModel
            {
                TwoFactorType = (TwoFactor)CurrentUser.TwoFactor
            };

            ViewBag.TwoFactorTypes = new SelectList(Enum.GetNames(typeof(TwoFactor)));

            return View(authenticationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorAuth(AuthenticationViewModel model)
        {
            if (model.TwoFactorType == TwoFactor.MicrosoftGoogle)
                return RedirectToAction(nameof(TwoFactorWithAuthenticator));

            sbyte twoFactorType = (sbyte)model.TwoFactorType;

            CurrentUser.TwoFactor = twoFactorType;
            CurrentUser.TwoFactorEnabled = twoFactorType != 0; //0 ise "hiçbiri seçmiştir."

            await _userManager.UpdateAsync(CurrentUser);

            return RedirectToAction(nameof(TwoFactorAuth));
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorWithAuthenticator()
        {
            string unFormattedKey = await _userManager.GetAuthenticatorKeyAsync(CurrentUser); //Key varmı kontrol et.

            if (string.IsNullOrEmpty(unFormattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(CurrentUser); //key oluştur

                unFormattedKey = await _userManager.GetAuthenticatorKeyAsync(CurrentUser); //key'i al.
            }

            string _authenticatorUri = _twoFactorService.GenerateQrCodeUri(CurrentUser.Email, unFormattedKey);

            var authenticationViewModel = new AuthenticationViewModel
            {
                SharedKey = unFormattedKey,
                AuthenticatorUri = _authenticatorUri,
            };

            return View(authenticationViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorWithAuthenticator(AuthenticationViewModel model)
        {
            string verificationCode = model.VerificationCode.Replace(" ", string.Empty).Replace("-", string.Empty); //Doğrulama kodunu kullanıcı yanlış yazabilir, temiz hale getir.

            string tokenProvider = _userManager.Options.Tokens.AuthenticatorTokenProvider; //Token Provider'ı al.

            bool twoFactorTokenValid = await _userManager.VerifyTwoFactorTokenAsync(CurrentUser, tokenProvider, verificationCode);

            if (!twoFactorTokenValid)
            {
                ModelState.AddModelError(string.Empty, ErrorMessage.VERIFICATION_CODE_NOT_VALID);
                return View(model);
            }

            CurrentUser.TwoFactorEnabled = true;
            CurrentUser.TwoFactor = (sbyte)TwoFactor.MicrosoftGoogle;

            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(CurrentUser, Identity.RECOVERY_CODE_COUNT); //5 Adet sıfırlama kodu üret.
            TempData[Namer.RECOVERY_CODES] = recoveryCodes;

            return RedirectToAction(nameof(TwoFactorAuth));
        }

    }
}
