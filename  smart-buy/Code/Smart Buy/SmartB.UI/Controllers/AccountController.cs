using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using SmartB.UI.Helper;
using SmartB.UI.Infrastructure;
using WebMatrix.WebData;
using SmartB.UI.Filters;
using SmartB.UI.Models;
using SmartB.UI.Models.EntityFramework;
using System.Net;
using System.Data.Entity;
using PagedList;

namespace SmartB.UI.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private SmartBuyEntities context = new SmartBuyEntities();

        //
        // GET: /Account/Login
        
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Thông tin đăng nhập không hợp lệ.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

//        [HttpPost]
//        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            Session["Cart"] = null;

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    // Register member
                    model.RoleId = (int) SystemRole.Member;

                    var accountHelper = new AccountHelper();
                    accountHelper.CreateAccount(model);
                    //WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new {RoleId = 3, IsActive = true});
                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public RedirectToRouteResult ChangePassword()
        {
            return RedirectToAction("AccountDetails");
        }

        public ActionResult AccountDetails()
        {
            var user = context.Users
                .Include(x => x.Profile)
                .FirstOrDefault(x => x.Username == User.Identity.Name);
            if (user.Profile == null)
            {
                user.Profile = new Profile();
            }
            var markets = context.Markets
                .Where(x => x.IsActive)
                .ToList();
            var model = new AccountDetailModel
                            {
                                Username = User.Identity.Name,
                                Markets = markets,
                                FirstRouteName = user.Profile.FirstRouteName,
                                FirstRoute = user.Profile.FirstRoute,
                                FirstMarkets = user.Profile.FirstMarkets,
                                SecondRouteName = user.Profile.SecondRouteName,
                                SecondRoute = user.Profile.SecondRoute,
                                SecondMarkets = user.Profile.SecondMarkets,
                                ThirdRouteName = user.Profile.ThirdRouteName,
                                ThirdRoute = user.Profile.ThirdRoute,
                                ThirdMarkets = user.Profile.ThirdMarkets
                            };
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers
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

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Tài khoản đã tồn tại. Vui lòng chọn tên khác.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "Địa chỉ email đã tồn tại. Vui lòng chọn địa chỉ khác.";

                case MembershipCreateStatus.InvalidPassword:
                    return "Mật khẩu không hợp lệ. Vui lòng chọn mật khẩu khác.";

                case MembershipCreateStatus.InvalidEmail:
                    return "Địa chỉ email không hợp lệ. Vui lòng chọn địa chỉ khác.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
