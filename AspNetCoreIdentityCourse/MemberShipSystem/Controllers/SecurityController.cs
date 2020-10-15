﻿using MemberShip.Web.Models;
using MemberShip.Web.Tools;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MemberShip.Web.Controllers
{
    [AllowAnonymous]
    public class SecurityController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SecurityController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult SignIn(string returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) //kullanıcı var mı?
            {
                ModelState.AddModelError(nameof(model.Email), ErrorMessage.USER_NOT_FOUND);
                return View(model);
            }

            var isLockedUser = await _userManager.IsLockedOutAsync(user); //Kullanıcı kilitli mi?

            if (isLockedUser)
            {
                ModelState.AddModelError(string.Empty, ErrorMessage.USER_LOCKOUT);
                return View(model);
            }

            await _signInManager.SignOutAsync(); //Benim kullanıcı hakkında yazdığım herhangi bir cookie varsa, silinsin.

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (!signInResult.Succeeded)
            {
                await _userManager.AccessFailedAsync(user); //Hatalı giriş sayısını +1 arttır.
                int failedLogins = await _userManager.GetAccessFailedCountAsync(user); //Kullanıcının hatalı giriş sayısını arttır.

                if (failedLogins == 3)
                {
                    await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(20)));//Kullanıcının hesabını 20 dakika kilitle.
                    ModelState.AddModelError(string.Empty, ErrorMessage.INCORRECT_LOGIN);

                    return View(model);
                }

                ModelState.AddModelError(string.Empty, $"{ErrorMessage.FAILED_LOGIN} Kalan başarısız deneme hakkınız: {3 - failedLogins}");
                return View(model);
            }

            await _userManager.ResetAccessFailedCountAsync(user); //Kullanıcının başarılı giriş yaptığı için hatalı giriş sayısını sıfırla.

            if (TempData["ReturnUrl"] != null)
                return Redirect(TempData["ReturnUrl"] as string);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var appUser = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            IdentityResult result = await _userManager.CreateAsync(appUser, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(nameof(model.Email), ErrorMessage.USER_NOT_FOUND);
                return View(model);
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user); // Şifre sıfırlama token'ını oluşturuyoruz.

            string passwordResetLink = Url.Action(nameof(ResetPasswordConfirm), "Security", new
            {
                userId = user.Id,
                token = passwordResetToken
            }, HttpContext.Request.Protocol); // Şifre sıfırlama linki oluşturuldu.

            Sender.PasswordResetMail(passwordResetLink, user.Email); //Şifre sıfırlama maili gönderildi.

            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return RedirectToAction(nameof(SignIn));

            var resetPasswordConfirmModel = new ResetPasswordConfirmModel()
            {
                UserId = userId,
                Token = token
            };

            return View(resetPasswordConfirmModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm(ResetPasswordConfirmModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return RedirectToAction(nameof(SignIn));

            IdentityResult identityResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword); //Token ile beraber şifre resetleme metotu.

            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            /*
            * Veri tabanında yer alan "SecurityStamp" kolonu kullanıcının o anki tüm bilgilerinin snapshot'ı olan bir kolondu. Şifremizi sıfırladıktan sonra
            * securitstamp alanını update etmemiz gerekli. Çünkü kullanıcı ile ilgili önemli bir bilgi değiştirdiğimizde security stamp'i güncellememiz gerekli.
            * 
            * Neden security stamp'i güncelliyoruz? Çünkü security stamp cookie içerisinde var. Default olarak identity mimamarisi 30 dakikada bir cookie'deki securitystamp ile
            * server'daki yani veritabanındaki securitystamp'i kıyaslar, bir uyuşmazlık yakalarsa kullanı oturumunu sonlandurır. Bu 30 dakika default değeri değiştirebiliriz.
            */

            await _userManager.UpdateSecurityStampAsync(user);

            return RedirectToAction(nameof(SignIn));
        }
    }
}