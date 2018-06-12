﻿using System.Web.Mvc;

namespace Store.Web
{
    public static class UrlHelperExtension
    {
        public static string GetProductImagePath(UrlHelper helper)
        {
            return helper.Content("~/Images/Products/");
        }

        public static MvcHtmlString ProductImagePath(this UrlHelper helper)
        {
            return MvcHtmlString.Create(GetProductImagePath(helper));
        }
    }
}