using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sportWorld.Models
{
    public class Category
    {

        [Key] // Primary Key
        public int Id { get; set; }
        [Required] // Not null
        [DisplayName("Category Name")] // Display label in UI 
        public string Name { get; set; }
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
