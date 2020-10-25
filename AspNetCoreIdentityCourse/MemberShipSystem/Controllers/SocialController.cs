using MemberShip.Web.Models;
using MemberShip.Web.Tools;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MemberShip.Web.Controllers
{
    [AllowAnonymous]
    public class SocialController : BaseController
    {
        public SocialController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
            : base(userManager, signInManager)
        {

        }

        [HttpGet]
        public IActionResult SignInWithFacebook(string ReturnUrl)
        {
            string redirectUrl = Url.Action(nameof(SignInWithSocialResponse), "Social", new { ReturnUrl = ReturnUrl });

            AuthenticationProperties authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);

            return new ChallengeResult("Facebook", authenticationProperties);
        }

        [HttpGet]
        public IActionResult SignInWithGoogle(string ReturnUrl)
        {
            string redirectUrl = Url.Action(nameof(SignInWithSocialResponse), "Social", new { ReturnUrl = ReturnUrl });

            AuthenticationProperties authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return new ChallengeResult("Google", authenticationProperties);
        }

        [HttpGet]
        public IActionResult SignInWithMicrosoft(string ReturnUrl)
        {
            string redirectUrl = Url.Action(nameof(SignInWithSocialResponse), "Social", new { ReturnUrl = ReturnUrl });

            AuthenticationProperties authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Microsoft", redirectUrl);

            return new ChallengeResult("Microsoft", authenticationProperties);
        }

        [HttpGet]
        public IActionResult SignInWithTwitter(string ReturnUrl)
        {
            string redirectUrl = Url.Action(nameof(SignInWithSocialResponse), "Social", new { ReturnUrl = ReturnUrl });

            AuthenticationProperties authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Twitter", redirectUrl);

            return new ChallengeResult("Twitter", authenticationProperties);
        }

        [HttpGet]
        public async Task<IActionResult> SignInWithSocialResponse(string ReturnUrl = "/Editor/Index")
        {
            List<string> errors = new List<string>(); //Hataları buna atacağım.

            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            /*
             * LoginProvider: Kullanıcı nereden kayıt olduysa orası yazak -> Facebook, Twitter, Microsoft gibi.
             * ProviderKey: Facebook userId, google userId gibi.
             */

            if (externalLoginInfo == null)
                return RedirectToAction("SignIn", "Security");

            Microsoft.AspNetCore.Identity.SignInResult signInResult =
                await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, true); //Kullanıcı kayıt olmuşsa bilgileri varsa direk giriş işlemi yaptırıyorum.

            if (signInResult.Succeeded) //dönlaha önce giriş yapmış, anasayfaya yendirebilirim.
                return RedirectToAction("Index", "Editor");

            //Eğer ilk kez giriş yapıyorsa

            var user = new AppUser();
            user.EmailConfirmed = true;
            user.Email = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email).Value; //Kullanıcının email adresi

            string externalUserId = externalLoginInfo.Principal.FindFirst(ClaimTypes.NameIdentifier).Value; //Kullanıcının user id si

            bool hasClaimName = externalLoginInfo.Principal.HasClaim(p => p.Type == ClaimTypes.Name); //Claim içerisinde name var mı?

            if (hasClaimName)
            {
                var fullName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Name).Value; //kullanıcının adı ve soyadı

                user.UserName = $"{fullName.Replace(" ", "").ToLower()}{externalUserId.Substring(0, 5)}"; //user id'sinden 5 karakter alıyorum ki çakışma olmasın.
            }
            else
            {
                user.UserName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email).Value; //Kullanıcının username i yoksa, email'i username olarak kullan.
            }
            
            var userWithMail = await _userManager.FindByEmailAsync(user.Email); //Email sistemde var mı?

            if (userWithMail == null) //Email sistemde kayıtlı değilse.
            {
                IdentityResult createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    errors = createResult.Errors.Select(p => p.Description).ToList();
                    return View(nameof(Error), errors);
                }

                IdentityResult loginResult = await _userManager.AddLoginAsync(user, externalLoginInfo); //Kullanıcıyı oluşturduktan sonra AspNetUserLogins tablosuna bilgileri ekliyorum.

                if (!loginResult.Succeeded)
                {
                    errors = loginResult.Errors.Select(p => p.Description).ToList();
                    return View(nameof(Error), errors);
                }
            }
            else
            {
                //email adresine ait veritabanımda kullanıcı var ise

                IdentityResult loginResult = await _userManager.AddLoginAsync(userWithMail, externalLoginInfo); //emaile sahip olan kullanıcının bilgilerini ekliyorum.

                if (!loginResult.Succeeded)
                {
                    errors = loginResult.Errors.Select(p => p.Description).ToList();
                    return View(nameof(Error), errors);
                }
            }

            signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, true); //Kullanıcıyı giriş yaptır.

            if (!signInResult.Succeeded)
            {
                errors.Add(ErrorMessage.LOGIN_ERROR);
                return View(nameof(Error), errors);
            }

            var asd = User.Identity.Name;

            //Burdan sonrası sayfa yönlendirme. Uğraşmak istemedim.

            if (string.IsNullOrEmpty(ReturnUrl))
                return Redirect(ReturnUrl);

            return RedirectToAction("Index", "Editor");
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}
