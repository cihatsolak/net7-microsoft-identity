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

    }
}

