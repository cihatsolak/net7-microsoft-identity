using MemberShip.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Controllers
{
    [Authorize(Roles = Role.ADMIN + "," + Role.MANAGER)]
    public class ManagerController : BaseController
    {
        public ManagerController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager) 
            : base(userManager, signInManager, roleManager)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
