namespace AspNetCoreIdentityApp.Web.Seeds
{
    public static class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            var hasBasicRole = await roleManager.RoleExistsAsync("Basic");
            var hasAdvancedRole = await roleManager.RoleExistsAsync("Advanced");
            var hasAdminRole = await roleManager.RoleExistsAsync("Admin");

            if (!hasBasicRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "Basic" });

                var basicRole = await roleManager.FindByNameAsync("Basic");

                await AddReadPermission(basicRole, roleManager);
            }

            if (!hasAdvancedRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "Advanced" });

                var advancedRole = await roleManager.FindByNameAsync("Advanced");

                await AddReadPermission(advancedRole, roleManager);
                await AddUpdateAndCreatePermission(advancedRole, roleManager);
            }


            if (!hasAdminRole)
            {
                await roleManager.CreateAsync(new AppRole() { Name = "Admin" });

                var basicRole = await roleManager.FindByNameAsync("Admin");

                await AddReadPermission(basicRole, roleManager);
                await AddUpdateAndCreatePermission(basicRole, roleManager);
                await AddDeletePermission(basicRole, roleManager);
            }
        }

        public static async Task AddReadPermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Stock.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Order.Read));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Catalog.Read));

        }

        public static async Task AddUpdateAndCreatePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Stock.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Order.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Catalog.Create));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Stock.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Order.Update));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Catalog.Update));
        }

        public static async Task AddDeletePermission(AppRole role, RoleManager<AppRole> roleManager)
        {
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Stock.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Order.Delete));
            await roleManager.AddClaimAsync(role, new Claim("Permission", Permissions.Catalog.Delete));
        }
    }
}