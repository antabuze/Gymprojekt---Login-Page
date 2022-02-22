using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SysAdminLogin.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [MaxLength(11)]
        public string SocialSecurityNumber { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(50)]
        public string LastName { get; set; }
        [MaxLength(11)]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string Password { get; set; }
        public string RoleId { get; set; }
    }
}
