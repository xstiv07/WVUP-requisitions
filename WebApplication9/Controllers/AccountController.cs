using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using IdentitySample.Models;
using WebApplication9.Data;
using Postal;
using System;

namespace testing111.Controllers
{
    [Authorize]
    [RequireHttps]
    public class AccountController : Controller
    {
        private MyUserManager _userManager;
        public AccountController()
        {
        }

        public AccountController(MyUserManager userManager)
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
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);

                if (user != null)
                {
                    //prevent users without confirmed emails to log in
                    if (!(await UserManager.IsEmailConfirmedAsync(user.Id)))
                    {
                        return View("ConfirmEmailAccount");
                    }
                        
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToAction("Create", "Requisition");
                }
                else
                    ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult doesUserNameExists(string userName)
        {
            var user = UserManager.FindByName(userName);
            return Json(user == null);
        }

        [AllowAnonymous]
        [HttpPost]
        public JsonResult doesEmailExists(string Email)
        {
            var email = UserManager.FindByEmail(Email);
            return Json(email == null);
        }

        public ActionResult SendConfirm()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendConfirm(EmailConfirmationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || (await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is confirmed
                    return View("DisplayEmail");
                }

                var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                await GenerateEmail("ConfirmEmail", user, callbackUrl);

                return View("DisplayEmail");
            }
            return View("ConfirmEmailAccount", model);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Register()
        {
            if (Request.IsAjaxRequest())
                return PartialView();
            
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new MyUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    First_Name = model.First_Name,
                    Last_Name = model.Last_Name
                };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    await GenerateEmail("ConfirmEmail", user, callbackUrl);

                    return View("DisplayEmail");
                }
                AddErrors(result);
            }
            return View("Login");
        }

        private static async Task GenerateEmail(string name, MyUser user, string callbackUrl)
        {
            dynamic email = new Email(name);
            email.To = user.Email;
            email.From = "xstiv07@gmail.com"; //purchasing department email goes here
            email.Recepient = user.First_Name;
            email.CallbackUrl = callbackUrl;
            await email.SendAsync();
        }

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public ActionResult ConfirmEmail(long userId, string code)
        {
            if (userId == 0 || code == null)
                return View("ConfirmEmailAccount");

            var result = UserManager.ConfirmEmail(userId, code);
            
            //if email is confirmed
            if (result.Succeeded)
                return View();

            return View("ConfirmEmailAccount");
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.EmailForgot);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                await GenerateEmail("ResetPassword", user, callbackUrl);

                return View("ForgotPasswordConfirmation");
            }

            return View("Login", model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist

                return RedirectToAction("ResetPasswordConfirmation", "Account");

            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            var result = await UserManager.RemoveLoginAsync(long.Parse(User.Identity.GetUserId()), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(long.Parse(User.Identity.GetUserId()));
                await SignInAsync(user, false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ChangePasswordViewModel model)
        {
            var hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    var result = await UserManager.ChangePasswordAsync(long.Parse(User.Identity.GetUserId()), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(long.Parse(User.Identity.GetUserId()));
                        await SignInAsync(user, false);
                        return RedirectToAction(
                            "Manage",
                            new
                            {
                                Message = ManageMessageId.ChangePasswordSuccess
                            });
                    }
                    AddErrors(result);
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    var result = await UserManager.AddPasswordAsync(long.Parse(User.Identity.GetUserId()), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(
                            "Manage",
                            new
                            {
                                Message = ManageMessageId.SetPasswordSuccess
                            });
                    }
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(MyUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
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

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}