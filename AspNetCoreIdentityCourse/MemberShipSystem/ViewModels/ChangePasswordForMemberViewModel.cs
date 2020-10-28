using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.ViewModels
{
    public class ChangePasswordForMemberViewModel
    {
        [HiddenInput]
        [Required]
        public string UserId { get; set; }
        public string UserName { get; set; }

        [Display(Name = "Yeni Şifre")]
        [Required(ErrorMessage = "Yeni şifre gereklidir.")]
        [MinLength(4, ErrorMessage = "Şifre en az 4 karakter içermelidir.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "(Tekrar) Yeni Şifre")]
        [Required(ErrorMessage = "Yeni şifren gereklidir.")]
        [MinLength(4, ErrorMessage = "Şifre en az 4 karakter içermelidir.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Yeni şifre uyuşmuyor.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
