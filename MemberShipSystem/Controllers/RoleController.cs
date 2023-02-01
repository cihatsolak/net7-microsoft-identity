using Mapster;
using MemberShip.Web.Models;
using MemberShip.Web.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberShip.Web.Controllers
{
    public class RoleController : BaseController
    {
        public RoleController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
            : base(userManager, null, roleManager)
        {
        }

        [HttpGet]
        public IActionResult List()
        {
            var roles = _roleManager.Roles.ToListAsync().Result;
            var roleViewModel = roles.Adapt<List<RoleViewModel>>();
            return View(roleViewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
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

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public IActionResult Edit(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return RedirectToAction(nameof(List));

            var role = _roleManager.FindByIdAsync(roleId).Result;
            var roleViewModel = role.Adapt<RoleViewModel>();

            return View(roleViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleViewModel model)
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

            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
                return RedirectToAction(nameof(List));

            var appRole = await _roleManager.FindByIdAsync(roleId);

            if (appRole == null)
                return RedirectToAction(nameof(List));

            await _roleManager.DeleteAsync(appRole);

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public IActionResult Assign(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;

            if (user == null)
                return RedirectToAction(nameof(List));

            IList<string> rolesForUser = _userManager.GetRolesAsync(user).Result;

            var assignedRoles = new List<RoleAssignViewModel>();

            foreach (var role in GetRoles) //GetRoles BaseController'dan geliyor.
            {
                var roleAssignViewModel = new RoleAssignViewModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    Exist = rolesForUser.Any(p => p.Equals(role.Name)) //Bu rol, kullanıcıya atanmış roller arasında mı?
                };

                assignedRoles.Add(roleAssignViewModel);
            }

            ViewBag.Username = user.UserName;
            HttpContext.Session.SetString("userId", userId); //Post işlemi için aldım.

            return View(assignedRoles);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(List<RoleAssignViewModel> rolesAssignViewModel)
        {
            string userId = HttpContext.Session.GetString("userId");
            bool transactionStatus = true;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return RedirectToAction(nameof(List));

            var userRoles = await _userManager.GetRolesAsync(user); //Kullanıcının hazırdaki rolleri.

            IEnumerable<string> rolesToAssigned = null;
            IEnumerable<string> rolesToRemoved = null;

            if (userRoles.Any())
            {
                rolesToAssigned = rolesAssignViewModel.Where(p => p.Exist && userRoles.Any(name => name != p.Name)).
                                                Select(p => p.Name); //Eklenecek Roller

                rolesToRemoved = rolesAssignViewModel.Where(p => !p.Exist && userRoles.Any(name => name.Equals(p.Name)))
                                                    .Select(p => p.Name); //Kaldırılacak Roller
            }
            else
            {
                rolesToAssigned = rolesAssignViewModel.Where(p => p.Exist).Select(p => p.Name);
            }

            if (rolesToAssigned != null && rolesToAssigned.Any())
            {
                IdentityResult result = await _userManager.AddToRolesAsync(user, rolesToAssigned);

                if (!result.Succeeded)
                {
                    AddModelErrors(result);
                    transactionStatus = false;
                }
            }

            if (rolesToRemoved != null && rolesToRemoved.Any())
            {
                IdentityResult result = await _userManager.RemoveFromRolesAsync(user, rolesToRemoved);

                if (!result.Succeeded)
                {
                    AddModelErrors(result);
                    transactionStatus = false;
                }
            }

            if (!transactionStatus)
                return View(rolesAssignViewModel);


            return RedirectToAction(nameof(Assign), new { userId = user.Id });
        }
    }
}
