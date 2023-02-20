namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    /// <summary>
    /// Startup dosyasında container'a eklemelisin.
    /// </summary>
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            List<IdentityError> errors = new();

            var isDigit = int.TryParse(user.UserName[0].ToString(), out _);
            if (isDigit)
            {
                errors.Add(new IdentityError()
                {
                    Code = "UserNameContainFirstLetterDigit",
                    Description = "Kullanıcı adının ilk karekteri sayısal bir karakter içeremez"
                });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
