using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using WebApplication9.Data.Helpers;

namespace WebApplication9.Data
{
    public class ItemMetadata
    {

        [Display(Name = "Can be Bought From")]
        public string BuyFrom;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        public string ItemCategoryId;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression(@"^[+]?([0-9]+(?:[\.][0-9]*)?|\.[0-9]+)$", ErrorMessage = "Invalid number")]
        public string Quantity;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression(@"^[+]?([0-9]+(?:[\.][0-9]*)?|\.[0-9]+)$", ErrorMessage = "Invalid number")]
        public string Price;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [RegularExpression(@"^.{50,}$", ErrorMessage = "Minimum 50 characters required")]
        public string Description;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Justification Statement")]
        [RegularExpression(@"^.{50,}$", ErrorMessage = "Minimum 50 characters required")]
        public string Just_Statement;
    }
}
