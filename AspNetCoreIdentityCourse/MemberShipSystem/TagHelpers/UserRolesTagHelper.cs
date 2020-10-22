using MemberShip.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberShip.Web.TagHelpers
{
    [HtmlTargetElement("user-roles")]
    public class UserRolesTagHelper : TagHelper
    {
        public string UserId { get; set; }

        private readonly UserManager<AppUser> _userManager;
        public UserRolesTagHelper(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(UserId))
                await base.ProcessAsync(context, output);

            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
                await base.ProcessAsync(context, output);


            IEnumerable<string> userRolesName = await _userManager.GetRolesAsync(user);

            string htmlContent = string.Empty;

            userRolesName.ToList().ForEach(roleName =>
            {
                htmlContent += $"<span class='badge badge-info ml-1'>{roleName}</span>";
            });


            output.Content.SetHtmlContent(htmlContent);
        }
    }
}
