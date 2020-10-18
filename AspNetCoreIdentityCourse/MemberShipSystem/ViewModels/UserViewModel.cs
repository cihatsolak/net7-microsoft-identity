using MemberShip.Web.Tools.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Kullanıcı ismi gereklidir.")]
        [DisplayName("Kullanıcı Adı")]
        public string UserName { get; set; }

        [DisplayName("Email Adresiniz")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Required(ErrorMessage = "Lütfen e-posta adresi giriniz.")]
        public string Email { get; set; }

        [DisplayName("Telefon Numarası")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DisplayName("Doğum Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime? BirthDate { get; set; }

        [DisplayName("Fotoğraf")]
        [DataType(DataType.Upload)]
        public string Picture { get; set; }

        [DisplayName("Şehir")]
        [DataType(DataType.Text)]
        public string CityName { get; set; }

        [DisplayName("Cinsiyet")]
        public Gender Gender { get; set; }
    }
}
