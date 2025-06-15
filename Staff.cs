using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giles_Chen_test_1
{
    public class Staff
    {
        [Column("staffID")]
        public int staffID { get; set; }

        [Column("password")]
        public string password { get; set; }
    }
}
