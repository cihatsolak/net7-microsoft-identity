using MemberShip.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class BaseController : Controller
    {
        protected readonly UserManager<AppUser> _userManager;
        protected readonly SignInManager<AppUser> _signInManager;
        protected readonly RoleManager<AppRole> _roleManager;

        protected AppUser CurrentUser => _userManager.FindByNameAsync(User.Identity.Name).Result;
        protected List<AppRole> GetRoles => _roleManager.Roles.ToListAsync().Result;

        public BaseController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager = null, RoleManager<AppRole> roleManager = null)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        protected void AddModelErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public async void SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        [NonAction]
        protected string ReturnHomePageUrl()
        {
            var user = _signInManager.GetTwoFactorAuthenticationUserAsync().Result;

            if (user == null)
            {
                if (User.Identity.Name == null)
                {
                    return "/";
                }

                if (CurrentUser == null)
                {
                    return "/";
                }
            }

            string returnUrl = TempData[Namer.RETURN_URL] as string;

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return returnUrl;
            }

            var rolesLoggedUser = _userManager.GetRolesAsync(user).Result; //Giriş yapan kullanıcının rolleri

            if (rolesLoggedUser.Contains(Role.ADMIN))
            {
                return Url.Action(Namer.INDEX, Role.ADMIN);
            }
            else if (rolesLoggedUser.Contains(Role.MANAGER))
            {
                return Url.Action(Namer.INDEX, Role.MANAGER);
            }
            else
            {
                return Url.Action(Namer.INDEX, Role.MEMBER);
            }
        }
    }
}
