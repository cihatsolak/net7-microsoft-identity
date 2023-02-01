using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace MemberShip.Web.Tools.Enums
{
    public enum TwoFactor
    {
        [Display(Name = "Hiç biri")]
        None = 0,

        [Display(Name = "Telefon ile kimlik doğrulama")]
        Phone = 1,

        [Display(Name = "E-Posta ile kimlik doğrulama")]
        Email = 2,

        [Display(Name = "Microsoft/Google Authenticator ile kimlik doğrulama")]
        MicrosoftGoogle = 3
    }
}
