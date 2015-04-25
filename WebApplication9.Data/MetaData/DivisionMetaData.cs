using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WebApplication9.Data.MetaData
{
    public class DivisionMetaData
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Division")]
        public string Name;

        [Display(Name = "Division")]
        public string Id;
    }
}