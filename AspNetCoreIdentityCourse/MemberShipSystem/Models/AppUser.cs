using Microsoft.AspNetCore.Identity;
using System;

namespace MemberShip.Web.Models
{
    public class AppUser : IdentityUser
    {
        public string CityName { get; set; }
        public string Picture { get; set; }
        public DateTime? BirthDate { get; set; }
        public int Gender { get; set; }
    }
}
