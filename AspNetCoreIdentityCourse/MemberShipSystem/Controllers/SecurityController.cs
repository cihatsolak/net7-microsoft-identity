using MemberShip.Web.Models;
using MemberShip.Web.Tools;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Controllers
{
    [AllowAnonymous]
    public class SecurityController : BaseController
    {
        public SecurityController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(userManager, signInManager)
        {
        }

        [HttpGet]
        public IActionResult SignIn(string returnUrl)
        {
            if (User.Identity.IsAuthenticated) //Kullanıcı hali hazırda giriş yapmışsa
                return RedirectToAction("Index", "Editor");

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

            bool isEmailVerified = await _userManager.IsEmailConfirmedAsync(user);

            if (!isEmailVerified)
            {
                ModelState.AddModelError(nameof(model.Email), ErrorMessage.EMAIL_NOT_VERIFIED);
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

            var rolesLoggedUser = await _userManager.GetRolesAsync(user); //Giriş yapan kullanıcının rolleri

            var returnUrl = TempData[Namer.RETURN_URL] as string;

            if (rolesLoggedUser.Contains(Role.ADMIN))
            {
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(Namer.INDEX, Role.ADMIN);
            }
            else if (rolesLoggedUser.Contains(Role.MANAGER))
            {
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(Namer.INDEX, Role.MANAGER);
            }
            else
            {
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(Namer.INDEX, Role.EDITOR);
            }
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            if (User.Identity.IsAuthenticated) //Kullanıcı hali hazırda giriş yapmışsa
                return RedirectToAction("Index", "Editor");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            IdentityResult result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                AddModelErrors(result);
                return View(model);
            }

            //Kullnıcıyı kayıt ettiysem email doğrulama maili göndereceğim.

            string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string verificationLink = Url.Action(nameof(VerificationForEmail), "Security", new //link oluştur
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
