using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using WebApplication9.Data.Helpers;

namespace WebApplication9.Data.MetaData
{
    public class RequisitionMetadata
    {
        [Required(ErrorMessage = "Required")]
        public int DepartmentId;

        [Required(ErrorMessage = "Required")]
        public int AccountId;

        [Display(Name = "Requisitioned By")]
        public string Requisitioned_By;

        [Display(Name = "Date Submitted")]
        public string Date_Created;

        [Display(Name = "Dept. Manager")]
        public string Decision_By;

        [Display(Name = "Dept. Manager Explanation")]
        public string Explanation { get; set; }

        [Display(Name = "CFO")]
        public string CFO_Decision_By { get; set; }

        [Display(Name = "CFO Explanation")]
        public string CFO_Explanation { get; set; }
    }
}
