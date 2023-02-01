using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [DisplayName("Email Adresiniz")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Required(ErrorMessage = "Lütfen e-posta adresi giriniz.")]
        public string Email { get; set; }
    }
}
