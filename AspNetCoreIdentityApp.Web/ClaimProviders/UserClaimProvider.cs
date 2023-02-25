namespace AspNetCoreIdentityApp.Web.ClaimProviders
{
    /// <summary>
    /// Sadece Authorize attribute'una sahip olan yerlerde çalışır
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

            var currentUser = await _userManager.FindByNameAsync(claimsIdentity.Name);

            if (string.IsNullOrEmpty(currentUser.City))
            {
                return principal;
            }

            if (principal.HasClaim(x => x.Type != "city"))
            {
                Claim cityClaim = new("city", currentUser.City);
                claimsIdentity.AddClaim(cityClaim);
            }

            return principal;
        }
    }
}
