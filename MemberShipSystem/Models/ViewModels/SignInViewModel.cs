using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.Models.ViewModels
{
    public class SignInViewModel
    {
        [DisplayName("Email Adresiniz")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Required(ErrorMessage = "Lütfen e-posta adresi giriniz.")]
        public string Email { get; set; }

        [DisplayName("Şifreniz")]
        [Required(ErrorMessage = "Lütfen şifrenizi giriniz.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
