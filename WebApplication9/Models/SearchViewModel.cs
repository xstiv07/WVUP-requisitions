using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication9.Data;
using WebApplication9.Data.Helpers;
using WebApplication9.Helpers;

namespace WebApplication9.Models
{
    public class SearchViewModel
    {
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Account Num")]
        public int AccountNumber { get; set; } //new

        [Display(Name = "Account Name")]
        public int AccountName { get; set; } //new

        [Display(Name = "Fund")]
        public int FundNumber { get; set; } //new

        [Display(Name = "Division")]
        public int DivisionName { get; set; } //new

        [Display(Name="Requisition Id")]
        public long RequisitionID { get; set; }

        [Display(Name="Requisitioned By")]
        public string RequisitionedBy { get; set; }
        
        [Display(Name="Total From")]
        public float TotalFrom { get; set; }
        
        [Display(Name="Total Up To")]
        public float TotalUpTo { get; set; }

        public StatusEnum Status { get; set; }

        [Display(Name="Date Submitted From")]
        public DateTime Date_Submitted_From { get; set; }

        [Display(Name = "Date Submitted Up To")]
        public DateTime Date_Submitted_To { get; set; }

    }

}