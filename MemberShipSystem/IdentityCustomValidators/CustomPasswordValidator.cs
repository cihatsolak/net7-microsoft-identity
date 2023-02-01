using MemberShip.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MemberShip.Web.IdentityCustomValidators
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            var identityErrors = new List<IdentityError>();

            if (password.ToLower().Contains(user.UserName.ToLower())) //Şifre kullanıcı adını içeriyorsa.
            {
                identityErrors.Add(new IdentityError
                {
                    Code = "PasswordContainsUserName",
                    Description = "Girilen şifre kullanıcı adını içeremez."
                });
            }

            if (password.ToLower().Contains(user.Email.ToLower())) //Şifre kullanıcı adını içeriyorsa.
            {
                identityErrors.Add(new IdentityError
                {
                    Code = "PasswordContainsEmail",
                    Description = "Girilen şifre e-posta adresini içeremez."
                });
            }

            if (password.ToLower().Contains("1234"))
            {
                identityErrors.Add(new IdentityError
                {
                    Code = "PasswordContains1234",
                    Description = "Şifre alanı 1234 gibi bir alan içeremez."
                });
            }

            if (identityErrors.Count == 0)
                return Task.FromResult(IdentityResult.Success);
            else
                return Task.FromResult(IdentityResult.Failed(identityErrors.ToArray()));
        }
    }
}
