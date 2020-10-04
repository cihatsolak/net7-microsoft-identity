using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Kullanıcı ismi gereklidir.")]
        [DisplayName("Kullanıcı Adı")]
        public string UserName { get; set; }

        [DisplayName("Telefon Numarası")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        [DisplayName("Email Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifreniz gereklidir.")]
        [DisplayName("Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
