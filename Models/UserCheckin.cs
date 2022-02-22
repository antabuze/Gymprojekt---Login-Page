using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysAdminLogin.Models
{
    public class UserCheckin
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
