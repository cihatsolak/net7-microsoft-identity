using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MemberShip.Web.Tools.Constants.IdentityConstants;

namespace MemberShip.Web.Requirements
{
    public class ExpireDateExchangeRequirement : IAuthorizationRequirement
    {
    }

    public class ExpireDateExchangeHandle : AuthorizationHandler<ExpireDateExchangeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExpireDateExchangeRequirement requirement)
        {
            if (context.User == null && context.User.Identity == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var claim = context.User.Claims.
                    FirstOrDefault(p => p.Type.Equals(ClaimName.EXPIRE_DATE_EXCHANGE) && !string.IsNullOrEmpty(p.Value));

            if (claim == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (DateTime.Now < Convert.ToDateTime(claim.Value)) //Geçmiş bir tarih değilse.
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
