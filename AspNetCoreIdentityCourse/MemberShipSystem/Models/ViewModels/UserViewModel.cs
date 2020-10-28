using MemberShip.Web.Tools.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı ismi gereklidir.")]
        [DisplayName("Kullanıcı Adı")]
        public string UserName { get; set; }

        [DisplayName("Email Adresiniz")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Required(ErrorMessage = "Lütfen e-posta adresi giriniz.")]
        public string Email { get; set; }

        [DisplayName("Telefon Numarası")]
        [RegularExpression(@"^(0(\d{3}) (\d{3}) (\d{2}) (\d{2}))$", ErrorMessage = "Telefon numarası uygun formatta değil.")]
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
