using Mapster;
using MemberShip.Web.Models;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Controllers
{
    //[Authorize(Roles = Role.ADMIN)]
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
            : base(userManager, signInManager, roleManager)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Claims()
        {
            var claims = User.Claims.ToList();
            return View(claims);
        }

        [HttpGet]
        public async Task<IActionResult> Members()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModel = users.Adapt<List<UserViewModel>>();
            return View(userViewModel);
        }

        [HttpGet]
        public IActionResult ChangePasswordForMember(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction(nameof(Members));

            var user = _userManager.FindByIdAsync(userId).Result;

            if (user == null)
                return RedirectToAction(nameof(Members));

            var changePasswordForMember = new ChangePasswordForMemberViewModel
            {
                UserId = user.Id,
                UserName = user.UserName
            };

            return View(changePasswordForMember);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordForMember(ChangePasswordForMemberViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);

            string token = await _userManager.GeneratePasswordResetTokenAsync(user); //Şifre değiştirmek için token ürettim.

            IdentityResult result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword); //Şifre sıfırlandı.

            if (!result.Succeeded)
            {
                AddModelErrors(result);
                return View(model);
            }

            await _userManager.UpdateSecurityStampAsync(user); //Veri tabanı ve kullanıcının tarayıcındaki security stamp değerini birbiriyle uyumlu hale getiriyorum.

            return RedirectToAction(nameof(Members));
        }
    }
}

