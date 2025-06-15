using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Giles_Chen_test_1
{
    [Table("Foodandbevs")]
    public class Foodandbev
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] 
        public int FoodandbevID { get; set; }
        [Column("FoodandbevName")]
        public string foodandbevName { get; set; }

        [Column("FoodandbevPrice")]
        public decimal foodandbevPrice { get; set; }

        [Column("FoodandbevDescription")]
        public string foodandbevDescription { get; set; }

        [Column("FoodandbevImagePath")]
        [MaxLength(255)]
        public string foodandbevImagePath { get; set; } = string.Empty; 

        [NotMapped] 
        public Image foodandbevImage { get; set; }



        [Column("Type")]
        public FBType Type { get; set; }


      

        private static Random random = new Random();
        private static HashSet<int> foodandbevIds = new HashSet<int>();

        private static int GeneratefoodandbevID()
        {
            int newID;
            do
            {
                newID = random.Next(10000, 100000);
            }
            while (foodandbevIds.Contains(newID));
            foodandbevIds.Add(newID);
            return newID;
        }

        public Foodandbev(FBType type)
        {
            Type = type;
            foodandbevName = "ABC";
            foodandbevDescription = "this food is created by coffer bean";
            foodandbevImagePath = "images/myfoodimage.jpg";
            FoodandbevID = GeneratefoodandbevID();
            foodandbevPrice = 0;
        }
    }

        public enum FBType
        {
            Food,
            Beverage
        }
}
