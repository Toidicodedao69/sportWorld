using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sportWorld.Models.ViewModels
{
    public class HomeVM
    {
		public IEnumerable<Product> productList { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> categoryList { get; set; }
		public string? nameFilter { get; set; }

		public string? categoryFilter { get; set; }
		public int pageNumber { get; set; }
		public int TotalPages { get; set; }
		public bool HasPreviousPage => pageNumber > 1;
		public bool HasNextPage => pageNumber < TotalPages;

	}
}
