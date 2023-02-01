using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.Models.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [DisplayName("Role Name")]
        [Required(ErrorMessage = "Rol adı gereklidir.")]
        public string Name { get; set; }
    }
}
