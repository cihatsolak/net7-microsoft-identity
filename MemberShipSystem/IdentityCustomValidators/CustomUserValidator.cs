using MemberShip.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberShip.Web.IdentityCustomValidators
{
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var identityErrors = new List<IdentityError>();

            char[] digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            if (digits.Any(p => p == user.UserName[0])) //Kullanıcı adının ilk karakteri rakam olamaz.
            {
                identityErrors.Add(new IdentityError
                {
                    Code = "UserNamefirstLetterDigitContains",
                    Description = "Kullanıcı adı sayısal karakterle başlayamaz."
                });
            }

            if (identityErrors.Count == 0)
                return Task.FromResult(IdentityResult.Success);
            else
                return Task.FromResult(IdentityResult.Failed(identityErrors.ToArray()));
        }
    }
}
