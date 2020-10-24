﻿using Mapster;
using MemberShip.Web.Models;
using MemberShip.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Controllers
{
    [Authorize(Roles = Role.ADMIN)]
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
        public async Task<IActionResult> Members()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModel = users.Adapt<List<UserViewModel>>();
            return View(userViewModel);
        }

    }
}

