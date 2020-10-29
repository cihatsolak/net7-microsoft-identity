using MemberShip.Web.Tools.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.Models.ViewModels
{
    public class TwoFactorSignInViewModel
    {
        [DisplayName("Doğrulama Kodu")]
        [Required(ErrorMessage = "Doğrulama kodu boş olamaz.")]
        [StringLength(8, ErrorMessage = "Doğrulama kodunuz en fazla 8 karakter içerebilir.")]
        public string VerificationCode { get; set; } = string.Empty;
        public bool RememberMe { get; set; }

        [DisplayName("Sıfırlama kodu gireceğim.")]
        public bool IsRecoverycode { get; set; }
        public TwoFactor TwoFactorType { get; set; }
    }
}
