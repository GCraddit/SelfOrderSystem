using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giles_Chen_test_1
{
    public class Order
    {
        [Key] 
        public Guid OrderID { get; set; }


        public DateTime OrderTime { get; set; }


        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal GetTotalAmount()
        {
            return OrderItems.Sum(item => item.GetTotalPrice());
        }
    }
}
