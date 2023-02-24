namespace AspNetCoreIdentityApp.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToList();

            return View(roles);
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });
            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Rol oluşturulmuştur.";
            return RedirectToAction(nameof(RolesController.Index));
        }

        
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);
            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }

            return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id, Name = roleToUpdate!.Name! });
        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);
            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }

            roleToUpdate.Name = request.Name;

            await _roleManager.UpdateAsync(roleToUpdate);

            ViewData["SuccessMessage"] = "Rol bilgisi güncellenmiştir";

            return View();
        }
    }
}
