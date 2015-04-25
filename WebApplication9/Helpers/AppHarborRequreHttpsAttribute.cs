using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication9.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true,
         AllowMultiple = false)]
    public class RequireHttpsAttribute : System.Web.Mvc.RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsSecureConnection)
            {
                return;
            }

            if (string.Equals(filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"],
                "https",
                StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                return;
            }

            HandleNonHttpsRequest(filterContext);
        }
    }
}