using Mapster;
using MemberShip.Web.Models;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemberShip.Web.Controllers
{
    public class AdminController : BaseController
    {
        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : base(userManager, null, roleManager)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Members()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModel = users.Adapt<List<UserViewModel>>();
            return View(userViewModel);
        }

        [HttpGet]
        public IActionResult Roles()
        {
            var roles = _roleManager.Roles.ToListAsync().Result;
            var roleViewModel = roles.Adapt<List<RoleViewModel>>();
            return View(roleViewModel);
        }

        [HttpGet]
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var appRole = new AppRole
            {
                Name = model.Name
            };

            IdentityResult result = await _roleManager.CreateAsync(appRole);

            if (!result.Succeeded)
            {
                AddModelErrors(result);
                return View(model);
            }

            return RedirectToAction(nameof(Roles));
        }

        [HttpGet]
        public IActionResult RoleEdit(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return RedirectToAction(nameof(Roles));

            var role = _roleManager.FindByIdAsync(roleId).Result;
            var roleViewModel = role.Adapt<RoleViewModel>();

            return View(roleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RoleEdit(RoleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var appRole = await _roleManager.FindByIdAsync(model.Id);
            appRole.Name = model.Name;

            IdentityResult result = await _roleManager.UpdateAsync(appRole);

            if (!result.Succeeded)
            {
                AddModelErrors(result);
                return View(model);
            }

            return RedirectToAction(nameof(Roles));
        }

        [HttpPost]
        public async Task<IActionResult> RoleDelete(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return RedirectToAction(nameof(Roles));

            var appRole = await _roleManager.FindByIdAsync(roleId);

            if (appRole == null)
                return RedirectToAction(nameof(Roles));

            await _roleManager.DeleteAsync(appRole);

            return RedirectToAction(nameof(Roles));
        }
    }
}

