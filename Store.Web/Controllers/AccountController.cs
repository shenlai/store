using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Store.Web.ViewModels;
using Store.Web.UserService;
using System.IO;
using Store.Web.ProductService;

namespace Store.Web.Controllers
{
    /*************关注 登陆 微软提供的方法**********************/
    [Authorize]
    [HandleError]
    public class AccountController : Controller
    {

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                /*此处访问的是重写了的BookStoreMembershipProvider.ValidateUser() service->repository */
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "用户名或密码不正确！");
                }
            }
            return View();

        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(UserAccountModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            try
            {
                MembershipCreateStatus createStatus = MembershipCreateStatus.ProviderError;

                var onlineStoreMembershipProvider = Membership.Provider as StoreMembershipProvider;
                if (onlineStoreMembershipProvider != null)
                    onlineStoreMembershipProvider.CreateUser(model.UserName,
                        model.Password,
                        model.Email,
                        null,
                        null,
                        true,
                        null,
                        model.Contact,
                        model.PhoneNumber,
                        new AddressDto
                    {
                        Country = model.ContactAddressCountry,
                        State = model.ContactAddressState,
                        City = model.ContactAddressCity,
                        Street = model.ContactAddressStreet,
                        Zip = model.ContactAddressZip
                    },
                    new AddressDto
                    {
                        Country = model.DeliveryAddressCountry,
                        State = model.DeliveryAddressState,
                        City = model.DeliveryAddressCity,
                        Street = model.DeliveryAddressStreet,
                        Zip = model.DeliveryAddressZip
                    },
                    out createStatus);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Home");
                }else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }

            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }
            return View(model);
        }

        #region Helpers
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "用户名已存在，请选择另一个用户名。";

                case MembershipCreateStatus.DuplicateEmail:
                    return "电子邮件地址已存在，请选择另一个电子邮件地址。";

                case MembershipCreateStatus.InvalidPassword:
                    return "输入的密码不正确，请重新输入正确的密码。";

                case MembershipCreateStatus.InvalidEmail:
                    return "输入的电子邮件地址不正确，请输入正确的电子邮件地址。";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "输入的用户名不正确，请输入正确的用户名。";

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