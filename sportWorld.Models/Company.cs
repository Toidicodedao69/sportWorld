using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sportWorld.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }
		[Required]
		[Display(Name = "Company Name")]
		public string Name { get; set; }
		[Display(Name="Street Address")]
		public string? StreetAddress { get; set; }
		public string? City { get; set; }
		public string? State { get; set; }
		[DisplayName("Postal Code")]
		public string? PostalCode { get; set; }
		[Display(Name="Phone Number")]
		public string? PhoneNumber { get; set; }
	}
}
