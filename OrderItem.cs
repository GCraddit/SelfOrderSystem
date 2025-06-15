using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Giles_Chen_test_1
{
    public class OrderItem
    {
        [Key] 
        public Guid OrderItemID { get; set; }

        [ForeignKey("Order")] 
        public Guid OrderID { get; set; }

        
        public virtual Order Order { get; set; }

        [ForeignKey("Foodandbev")] 
        public int FoodandbevID { get; set; }

       
        public virtual Foodandbev Foodandbev { get; set; }

      
        public int Quantity { get; set; }

        [NotMapped]
        public string foodandbevName => Foodandbev?.foodandbevName;

        [NotMapped]
        public decimal TotalPrice => Foodandbev != null ? Foodandbev.foodandbevPrice * Quantity : 0;


        public decimal GetTotalPrice()
        {
            return Foodandbev.foodandbevPrice * Quantity;
        }
    }
}
