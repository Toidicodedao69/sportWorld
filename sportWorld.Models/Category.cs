using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sportWorld.Models
{
    public class Category
    {

        [Key] // Primary Key
        public int Id { get; set; }
        [Required] // Not null
        [MaxLength(100)] // Validation
        [DisplayName("Category Name")] // Display label in UI 
        public string Name { get; set; }
        [Range(0, 100, ErrorMessage = "Display Order must be 1-100")] // Validation
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
