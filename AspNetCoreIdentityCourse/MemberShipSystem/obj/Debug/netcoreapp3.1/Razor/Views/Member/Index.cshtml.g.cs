#pragma checksum "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f9feb79c68fcfba3300da821a5a73861cc7d2aa2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Member_Index), @"mvc.1.0.view", @"/Views/Member/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\_ViewImports.cshtml"
using MemberShip.Web.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\_ViewImports.cshtml"
using MemberShip.Web.ViewModels;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\_ViewImports.cshtml"
using MemberShip.Web.Tools.Enums;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f9feb79c68fcfba3300da821a5a73861cc7d2aa2", @"/Views/Member/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"33217b40978eb38254146bea1027bbfb55f303d5", @"/Views/_ViewImports.cshtml")]
    public class Views_Member_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<UserViewModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
  
    ViewData["Title"] = "Index";
    Layout = "~/Views/Shared/_MemberLayout.cshtml";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h3 class=\"text-center\">User Information</h3>\r\n\r\n<div class=\"row\">\r\n    <div class=\"col-sm-3\">\r\n        <img");
            BeginWriteAttribute("src", " src=\"", 226, "\"", 246, 1);
#nullable restore
#line 11 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
WriteAttributeValue("", 232, Model.Picture, 232, 14, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" style=\"width:100%;height:100%\" />\r\n    </div>\r\n    <div class=\"col-sm-9\">\r\n        <table class=\"table table-bordered table-striped\">\r\n            <tr>\r\n                <th>User Name</th>\r\n                <td>");
#nullable restore
#line 17 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
               Write(Model.UserName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>E-Mail</th>\r\n                <td>");
#nullable restore
#line 21 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
               Write(Model.Email);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>Phone Number</th>\r\n                <td>");
#nullable restore
#line 25 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
               Write(Model.PhoneNumber);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n            </tr>\r\n            <tr>\r\n                <th>Birth Date</th>\r\n                <td>\r\n");
#nullable restore
#line 30 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                     if (Model.BirthDate != null)
                    {
                        

#line default
#line hidden
#nullable disable
#nullable restore
#line 32 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                   Write(Model.BirthDate);

#line default
#line hidden
#nullable disable
#nullable restore
#line 32 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                                        
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </td>\r\n            </tr>\r\n            <tr>\r\n                <th>City</th>\r\n                <td>\r\n");
#nullable restore
#line 39 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                     if (!string.IsNullOrEmpty(Model.CityName))
                    {
                        

#line default
#line hidden
#nullable disable
#nullable restore
#line 41 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                   Write(Model.CityName);

#line default
#line hidden
#nullable disable
#nullable restore
#line 41 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                                       
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </td>\r\n            </tr>\r\n            <tr>\r\n                <th>Gender</th>\r\n                <td>\r\n");
#nullable restore
#line 48 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                     switch (Model.Gender)
                    {
                        case Gender.Unspecified:

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <span>Belirtilmemiş</span>\r\n");
#nullable restore
#line 52 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                            break;
                        case Gender.Male:

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <span>Erkek</span>\r\n");
#nullable restore
#line 55 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                            break;
                        case Gender.Female:

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <span>Kadın</span>\r\n");
#nullable restore
#line 58 "C:\Users\CihatSolak\source\repos\cihatsolak\NetCoreIdentity\AspNetCoreIdentityCourse\MemberShipSystem\Views\Member\Index.cshtml"
                            break;
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </td>\r\n            </tr>\r\n        </table>\r\n    </div>\r\n</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<UserViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
