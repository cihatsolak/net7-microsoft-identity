using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Eski Şifreniz")]
        [Required(ErrorMessage = "Eski şifreniz gereklidir.")]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakter içermelidir.")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Display(Name = "Yeni Şifreniz")]
        [Required(ErrorMessage = "Yeni şifreniz gereklidir.")]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakter içermelidir.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "(Tekrar) Yeni Şifreniz")]
        [Required(ErrorMessage = "Yeni şifreniz gereklidir.")]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakter içermelidir.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Yeni şifreniz uyuşmuyor.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
