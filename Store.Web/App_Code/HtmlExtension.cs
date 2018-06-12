using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Web.Routing;
using Store.Web;
using Store.Web.UserService;


namespace System.Web.Mvc
{
    public static partial class HtmlExtension
    {
        #region ActionLinkWithPermission

        public static MvcHtmlString ActionLinkWithPermission(this HtmlHelper helper, string linkText, string action, string controller, PermissionKeys required)
        {
            if (helper == null ||
                helper.ViewContext == null ||
                helper.ViewContext.RequestContext == null ||
                helper.ViewContext.RequestContext.HttpContext == null ||
                helper.ViewContext.RequestContext.HttpContext.User == null ||
                helper.ViewContext.RequestContext.HttpContext.User.Identity == null)
                return MvcHtmlString.Empty;

            using (var proxy = new UserServiceClient())
            {
                var role = proxy.GetRoleByUserName(helper.ViewContext.HttpContext.User.Identity.Name);
                if (role == null)
                    return MvcHtmlString.Empty;
                var keyName = role.Name;
                var permissionKey = (PermissionKeys)Enum.Parse(typeof(PermissionKeys), keyName);

                // 通过用户的角色和对应对应的权限进行与操作
                // 与结果等于用户角色时，表示用户角色与所需要的权限一样，则创建对应权限的链接
                //permissionKey & required 按位与运算 (required 权限按位或结果，比如权限 1或2 = 0011 )
                return (permissionKey & required) == permissionKey ?
                    MvcHtmlString.Create(HtmlHelper.GenerateLink(helper.ViewContext.RequestContext, helper.RouteCollection,
                    linkText, null, action, controller, null, null)) : MvcHtmlString.Empty;
            }
        }

        #endregion
    }
}