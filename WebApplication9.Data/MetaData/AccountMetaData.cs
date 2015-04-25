using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WebApplication9.Data.MetaData
{
    public class AccountMetaData
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "DA")]
        public string String;

        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [Display(Name = "Charge To")]
        public string Name;

        [Required(ErrorMessage = "Required")]
        public int DivisionId;

        [Required(ErrorMessage = "Required")]
        public int DepartmentId;

        [Required(ErrorMessage = "Required")]
        public int FundId;
    }
}
