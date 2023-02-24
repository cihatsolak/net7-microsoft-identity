namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "master")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public HomeController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var userList = await _userManager.Users.Select(user => new AdminUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.UserName
            }).ToListAsync();

            return View(userList);
        }
    }
}
