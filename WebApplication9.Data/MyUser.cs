using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplication9.Data.Helpers;

namespace WebApplication9.Data
{
    public class MyUser : IdentityUser<long, MyLogin, MyUserRole, MyClaim>
    {
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
 
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(MyUserManager userManager)
        {
            var userIdentity = await userManager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
