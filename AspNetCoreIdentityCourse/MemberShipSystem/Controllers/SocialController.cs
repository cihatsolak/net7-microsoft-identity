using MemberShip.Web.Models;
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
            string redirectUrl = Url.Action(nameof(SignInWithFacebookResponse), new { ReturnUrl = ReturnUrl });

            AuthenticationProperties authenticationProperties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);

            return new ChallengeResult("Facebook", authenticationProperties);
        }

        [HttpGet]
        public async Task<IActionResult> SignInWithFacebookResponse(string ReturnUrl = "/")
        {
            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            /*
             * LoginProvider: Kullanıcı nereden kayıt olduysa orası yazak -> Facebook, Twitter, Microsoft gibi.
             * ProviderKey: Facebook userId, google userId gibi.
             */

            if (externalLoginInfo == null)
                return RedirectToAction("SignIn", "Security");

            Microsoft.AspNetCore.Identity.SignInResult signInResult =
                await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.LoginProvider, true);

            if (signInResult.Succeeded) //daha önce giriş yapmış, anasayfaya yönlendirebilirim.
                return Redirect(ReturnUrl);

            //Eğer ilk kez facebook login diyorsa

            var user = new AppUser();

            user.Email = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email).Value; //Kullanıcının facebook email adresi

            string facebookUserId = externalLoginInfo.Principal.FindFirst(ClaimTypes.NameIdentifier).Value; //Kullanıcının facebook user id si

            bool hasClaimName = externalLoginInfo.Principal.HasClaim(p => p.Type == ClaimTypes.Name); //Claim içerisinde name var mı?

            if (hasClaimName)
            {
                var fullName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Name).Value; //kullanıcının facebook da ki adı ve soyadı

                user.UserName = $"{fullName.Replace(" ", "").ToLower()}{facebookUserId.Substring(0, 5)}"; //facebook user id'sinden 5 karakter alıyorum ki çakışma olmasın.
            }
            else
            {
                user.UserName = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email).Value; //Kullanıcının username i yoksa, email'i username olarak kullan.
            }

            IdentityResult createResult = await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                List<string> errors = createResult.Errors.Select(p => p.Description).ToList();
                return View(nameof(Error), errors);
            }

            IdentityResult loginResult = await _userManager.AddLoginAsync(user, externalLoginInfo); //Kullanıcıyı oluşturduktan sonra AspNetUserLogins tablosuna bilgileri ekliyorum.

            if (!loginResult.Succeeded)
            {
                List<string> errors = createResult.Errors.Select(p => p.Description).ToList();
                return View(nameof(Error), errors);
            }

            await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.LoginProvider, true); //Kullanıcıyı giriş yaptır.

            return Redirect(ReturnUrl);
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}
