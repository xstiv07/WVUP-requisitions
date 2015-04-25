using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApplication9.Data
{
    public class MyRole : IdentityRole<long, MyUserRole>
    {
        public MyRole() { }

        public MyRole(string name)
            : this()
        {
            this.Name = name;
        }
    }
}
