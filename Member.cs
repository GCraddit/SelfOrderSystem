using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Giles_Chen_test_1
{
    public class Member
    {
        [Key]
        [Column("MemberId")]
        public int MemberId { get; set; }

        [Required]
        [Column("Phone")]
        public string Phone { get; set; }

        [Required]
        [Column("MbPassword")]
        public string MbPassword { get; set; }

        
        [Column("Name")]
        [Required]
        public string Name { get; set; }

        [Column("Point")]
        [Required]
        public int Point { get; set; } = 0;
    }
}
