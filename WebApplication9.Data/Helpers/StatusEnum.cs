using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication9.Data.Helpers
{
    public enum StatusEnum
    {
        Submitted = 1,
        Approved = 2,
        Rejected = 3,
        Complete = 4,
        Void = 5,

        [Display(Name="Approved by CFO")]
        Aproved_CFO = 6,

        [Display(Name="Rejected by CFO")]
        Rejected_CFO = 7,

        Ordered = 8
    }
}