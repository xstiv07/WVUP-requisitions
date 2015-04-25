using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WebApplication9.Data.MetaData
{
    public class DepartmentMetaData
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Department")]
        public String Name;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Division")]
        public int DivisionId;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Manager")]
        public int ManagerId;
    }
}
