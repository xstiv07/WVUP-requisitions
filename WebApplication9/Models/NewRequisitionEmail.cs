using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication9.Models
{
    public class NewRequisitionEmail : Email
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Recepient { get; set; }
        public string UserName { get; set; }
    }
}