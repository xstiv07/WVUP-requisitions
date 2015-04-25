using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WebApplication9.Data.MetaData
{
    public class RequisitionExtrasMetadata
    {
        [Display(Name = "Credit Card Holder's Name")]
        [Required(ErrorMessage = "Required")]
        public string CC_Holder_Name;

        [Display(Name = "Date Ordered")]
        [Required(ErrorMessage = "Required")]
        public DateTime? Date_Ordered;

        [Display(Name = "Expected Delivery Date")]
        public DateTime? Date_Expected_Delivery;

        [Display(Name = "Purchase Order Number")]
        public string Purchase_Order_Num;
    }
}
