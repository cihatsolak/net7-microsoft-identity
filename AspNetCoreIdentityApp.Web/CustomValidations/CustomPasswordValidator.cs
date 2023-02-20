namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    /// <summary>
    /// Startup dosyasında container'a eklemelisin.
    /// </summary>
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            var errors = new List<IdentityError>();

            if (password.Equals(user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainUserName",
                    Description = "Şifre alanı kullanıcı adı içeremez."
                });
            }

            if (password.StartsWith("1234"))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContain1234",
                    Description = "Şifre alanı ardışık sayı içeremez."
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
