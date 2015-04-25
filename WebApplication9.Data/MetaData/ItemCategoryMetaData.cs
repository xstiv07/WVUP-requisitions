using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WebApplication9.Data.MetaData
{
    public class ItemCategoryMetaData
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Category Name")]
        public string Name;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Category Number")]
        public string Number;
    }

}
