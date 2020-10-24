using MemberShip.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Threading.Tasks;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.ClaimProviders
{
    public class ClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> _userManager;
        public ClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal == null || principal.Identity.IsAuthenticated) //Giriş yapmış bir kullanıcı olmalı.
            {
                await Task.CompletedTask;
            }

            ClaimsIdentity claimsIdentity = principal.Identity as ClaimsIdentity; //Kullanıcının kimlik kartını alıyorum :)

            var user = await _userManager.FindByNameAsync(claimsIdentity.Name); //Geçerli kullanıcıyı buluyorum.

            if (user == null)
            {
                await Task.CompletedTask;
                return principal;
            }

            if (!string.IsNullOrEmpty(user.CityName)) //Kullanıcının şehir adının null olup olmadığına bakıyorum. (var mı yani?)
            {
                bool anyClaim = principal.HasClaim(p => p.Type.Equals(ClaimName.CITY)); //Şehir adı var tamam ama City adında bir claim e sahip mi?

                if (!anyClaim) //city adında bir claim e sahip değilse.
                {
                    Claim cityClaim = new Claim(ClaimName.CITY, user.CityName, ClaimValueTypes.String, ClaimName.ISSUER); //City için claim oluşturuyorum.
                    claimsIdentity.AddClaim(cityClaim);
                }
            }

            if (user.BirthDate != null)
            {
                int year = DateTime.Now.Year;
                int age = (int)(year - user.BirthDate?.Year);

                bool anyClaim = principal.HasClaim(p => p.Type.Equals(ClaimName.AGE)); //Yaş claim i var mı?

                if (!anyClaim) //yoksa
                {
                    Claim ageClaim = new Claim(ClaimName.AGE, age.ToString(), ClaimValueTypes.Integer, ClaimName.ISSUER);
                    claimsIdentity.AddClaim(ageClaim);
                }
            }

            return principal;
        }
    }
}
