using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sportWorld.Models
{
    public class Product
    {
        [Key] // Primary Key
        public int Id { get; set; }
        [Required] // Not null
        [DisplayName("Product Name")] // Display label in UI 
        public string Name { get; set; }
        public string Description { get; set; }
        public string Brand { get; set; }
        [Required]
        [Display(Name = "List Price")]
        [Range(0, 1000)]
        public double ListPrice { get; set; }
        [Required]
        [Display(Name="Price 1-20")]
        [Range(0, 1000)]
        public double Price { get; set; }

        [Display(Name = "Price 20+")]
        [Range(0, 1000)]
        public double Price20 { get; set; }
    }
}
