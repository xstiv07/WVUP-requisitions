using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication9.Data;
using WebApplication9.Data.Helpers;
using WebApplication9.Helpers;
using WebApplication9.Repository;

namespace IdentitySample.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        //Requisition1Entities db = new Requisition1Entities();

        private IRepository repo;
        private MyUserManager _userManager;
        public ManageController()
        {
        }

        public ManageController(MyUserManager userManager)
        {
            UserManager = userManager;
        }

        
        public MyUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<MyUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Index
        public async Task<ActionResult> Index()
        {
            var user = await GetCurrentUser();
            double result;
            var myRequisitions = repo.GetCurrentUserRequisitions(user);

            double positive = myRequisitions.Where(x => x.Status == StatusEnum.Complete || x.Status == StatusEnum.Approved || x.Status == StatusEnum.Aproved_CFO).ToList().Count;
            int voidReq = myRequisitions.Where(x => x.Status == StatusEnum.Void).ToList().Count;
            int rejected = myRequisitions.Where(x => x.Status == StatusEnum.Rejected || x.Status == StatusEnum.Rejected_CFO).ToList().Count;
            double total = positive + rejected;
            double totalWithVoid = myRequisitions.ToList().Count;

            if (positive > 0)
                result = Math.Round((positive / total) * 100, 1, MidpointRounding.AwayFromZero);
            else
                result = 0;
           
            var model = new IndexViewModel
            {
                Rejected = rejected,
                Void = voidReq,
                Result = result,
                TotalWithVoid = totalWithVoid,
            };
            return View(model);
        }

        private async Task<MyUser> GetCurrentUser()
        {
            long currentUserId = long.Parse(User.Identity.GetUserId());
            var user = await UserManager.FindByIdAsync(currentUserId);
            return user;
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(long.Parse(User.Identity.GetUserId()), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(long.Parse(User.Identity.GetUserId()));
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index");
            }
            AddErrors(result);
            return View(model);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(MyUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(long.Parse(User.Identity.GetUserId()));
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(long.Parse(User.Identity.GetUserId()));
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}