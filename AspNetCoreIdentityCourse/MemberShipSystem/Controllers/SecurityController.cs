using MemberShip.Web.Models;
using MemberShip.Web.Models.ViewModels;
using MemberShip.Web.Tools;
using MemberShip.Web.Tools.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Controllers
{
    [AllowAnonymous]
    public class SecurityController : BaseController
    {
        public SecurityController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            : base(userManager, signInManager)
        {
        }

        [HttpGet]
        public IActionResult SignIn(string returnUrl = "/")
        {
            if (User.Identity.IsAuthenticated) //Kullanıcı hali hazırda giriş yapmışsa
                return Redirect(returnUrl);

            TempData[Namer.RETURN_URL] = returnUrl;

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

            bool isEmailVerified = await _userManager.IsEmailConfirmedAsync(user); //Kullanıcı email adresini doğrulamış mı?

            if (!isEmailVerified)
            {
                ModelState.AddModelError(nameof(model.Email), ErrorMessage.EMAIL_NOT_VERIFIED);
                return View(model);
            }

            bool isCheckPassword = await _userManager.CheckPasswordAsync(user, model.Password); //Girilen şifre doğru mu?

            if (!isCheckPassword)
            {
                await _userManager.AccessFailedAsync(user); //Hatalı giriş sayısını +1 arttır.
                int failedCount = await _userManager.GetAccessFailedCountAsync(user); //Kullanıcının hatalı giriş sayısını getir.

                if (failedCount == Identity.FAILED_ENTRY_RIGHT) //Hatalı giriş sayısı 3 ise.
                {
                    await _userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddMinutes(20)));//Kullanıcının hesabını 20 dakika kilitle.
                    ModelState.AddModelError(string.Empty, ErrorMessage.INCORRECT_LOGIN);

                    return View(model);
                }

                ModelState.AddModelError(string.Empty, string.Format(ErrorMessage.FAILED_LOGIN_RIGHT_TRY, Identity.FAILED_ENTRY_RIGHT - failedCount));
                return View(model);
            }

            await _signInManager.SignOutAsync(); //Benim kullanıcı hakkında yazdığım herhangi bir cookie varsa, silinsin.
            await _userManager.ResetAccessFailedCountAsync(user); //Kullanıcının başarılı giriş yaptığı için hatalı giriş sayısını sıfırla.

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (signInResult.RequiresTwoFactor) //İki faktörlü doğrulama var ise. yani two faktör true ise.
            {
                return RedirectToAction(nameof(TwoFactorSignIn), new { rememberMe = model.RememberMe });
            }

            return Redirect(ReturnHomePageUrl());
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorSignIn(bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync(); //İki faktörlü doğrulamayı kullanan kullanıcıyı getir.

            if (user == null)
                return RedirectToAction(nameof(SignIn));

            var twoFactorSignInViewModel = new TwoFactorSignInViewModel
            {
                RememberMe = rememberMe,
                TwoFactorType = (TwoFactor)user.TwoFactor
            };

            return View(twoFactorSignInViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorSignIn(TwoFactorSignInViewModel model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync(); //İki faktörlü doğrulamayı kullanan kullanıcıyı getir.

            if (user == null)
                return RedirectToAction(nameof(SignIn));

            ModelState.Clear();

            Microsoft.AspNetCore.Identity.SignInResult signInResult = null;

            if ((TwoFactor)user.TwoFactor == TwoFactor.MicrosoftGoogle) //Google ya da Microsoft Authenticator kullanıyorsa?
            {
                if (model.IsRecoverycode) //Sıfırlama kodu girdiyse, kullanıcı telefonuna erişemiyorsa.
                {
                    signInResult = await _signInManager.TwoFactorRecoveryCodeSignInAsync(model.VerificationCode);
                }
                else
                {
                    signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(model.VerificationCode, model.RememberMe, false);
                    //Remember Client = Kullanıcı bir kez doğrulama kodu girdiğinde ve başarılı olduğunda cookie'ye kaydet ve bir daha doğrulama kodu isteme. Bu özellik güvenli olmadığından false verdik. 
                }

                if (!signInResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, ErrorMessage.VERIFICATION_CODE_NOT_MATCHED);
                    return View(model);
                }

                return Redirect(ReturnHomePageUrl());
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated) //Kullanıcı hali hazırda giriş yapmışsa
                return Redirect(ReturnHomePageUrl());

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool isRegisteredPhoneNumber = _userManager.Users.Any(p => p.PhoneNumber.Equals(model.PhoneNumber)); //Girelen telefon numarası kayıtlı mı

            if (isRegisteredPhoneNumber)
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), ErrorMessage.PHONE_NUMBER_USE);
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                TwoFactor = 0
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                AddModelErrors(result);
                return View(model);
            }

            //Kullnıcıyı kayıt ettiysem email doğrulama maili göndereceğim.

            string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string verificationLink = Url.Action(nameof(VerificationForEmail), Namer.SECURITY, new //link oluştur
            {
                UserId = user.Id,
                token = emailConfirmationToken

            }, protocol: HttpContext.Request.Scheme);

            Sender.EmailVerification(verificationLink, user.Email); //email gönderme

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

            string passwordResetLink = Url.Action(nameof(ResetPasswordConfirm), Namer.SECURITY, new
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

            IdentityResult result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword); //Token ile beraber şifre resetleme metotu.

            if (!result.Succeeded)
            {
                AddModelErrors(result);
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

        [HttpGet]
        public async Task<IActionResult> VerificationForEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return RedirectToAction(nameof(SignIn));

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return RedirectToAction(nameof(SignIn));

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                AddModelErrors(result);
                return RedirectToAction(nameof(SignIn));
            }

            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
