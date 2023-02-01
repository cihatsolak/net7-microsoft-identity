using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.Models.ViewModels
{
    public class ResetPasswordConfirmModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş geçilemez.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "(Tekrar) Şifre alanı boş geçilemez.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Şifreniz uyuşmamaktadır.")]
        [Display(Name = "(Tekrar) Yeni Şifre")]
        public string ReNewPassword { get; set; }
    }
}
