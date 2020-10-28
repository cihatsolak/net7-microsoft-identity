using MemberShip.Web.Tools.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.Models.ViewModels
{
    public class AuthenticationViewModel
    {
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }

        [DisplayName("Doğrulama Kodu"), DataType(DataType.Text)]
        [Required(ErrorMessage = "Doğrulama kodu gereklidi.")]
        public string VerificationCode { get; set; }

        [DisplayName("Doğrulama Türü")]
        public TwoFactor TwoFactorType { get; set; }
    }
}
