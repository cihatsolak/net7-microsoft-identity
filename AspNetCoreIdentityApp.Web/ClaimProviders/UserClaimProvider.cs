namespace AspNetCoreIdentityApp.Web.ClaimProviders
{
    /// <summary>
    /// Sadece Authorize attribute'una sahip olan yerlerde çalışır,
    /// Framework tarafından authorize attribute'üne sahip olunan alanlarda devreye girerek çalışır.
    /// </summary>
    public class UserClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;

        public UserClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var claimsIdentity = principal.Identity as ClaimsIdentity;

            var user = await _userManager.FindByNameAsync(claimsIdentity.Name);

            if (string.IsNullOrEmpty(user.City))
            {
                return principal;
            }

            if (principal.HasClaim(claim => claim.Type != "city"))
            {
                Claim cityClaim = new("city", user.City);
                claimsIdentity.AddClaim(cityClaim);
            }

            return principal;
        }
    }
}
