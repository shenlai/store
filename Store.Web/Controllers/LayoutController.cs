using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Store.Web.ProductService;
using System.Configuration;

namespace Store.Web.Controllers
{
    public class LayoutController : Controller
    {
        #region Shared Layout Partial view Actions

        /// <summary>
        /// 图书分类
        /// </summary>
        /// <returns></returns>
        public ActionResult CategoriesPartial()
        {
            using (var proxy = new ProductServiceClient())
            {
                var categories = proxy.GetCategories();
                return PartialView(categories);
            }
        }

        public ActionResult NewProductsPartial()
        {
            using (var proxy = new ProductServiceClient())
            {
                var newProducts = proxy.GetNewProducts(4);
                return PartialView(newProducts);
            }
        }

        //public ActionResult ProductsPartial(string categoryId = null)
        //{
        //    using (var proxy = new ProductServiceClient())
        //    {
        //        IEnumerable<ProductDto> products = null;
        //        products = string.IsNullOrEmpty((categoryId)) ? proxy.GetProducts() : proxy.GetProductsForCategory(new Guid(categoryId));
        //        if (string.IsNullOrEmpty(categoryId))
        //            ViewBag.CategoryName = "所有商品";
        //        else
        //        {
        //            var category = proxy.GetCategoryById(new Guid(categoryId));
        //            ViewBag.CategoryName = category.Name;
        //        }

        //        ViewBag.CategoryId = categoryId;
        //        return PartialView(products);
        //    }
        //}

        /// <summary>
        /// 商品页面的分页支持
        /// </summary>
        /// <param name="categoryId">类别Id</param>
        /// <param name="fromIndexPage">是否来源首页点击</param>
        /// <param name="pageNumber">页数</param>
        /// <returns></returns>
        public ActionResult ProductsPartial(string categoryId = null, bool? fromIndexPage = null, int pageNumber = 1)
        {
            using (var proxy = new ProductServiceClient())
            {
                var numberOfProductsPerPage = int.Parse(ConfigurationManager.AppSettings["productsPerPage"]);
                var pagination = new Pagination { PageSize = numberOfProductsPerPage, PageNumber = pageNumber };
                ProductDtoWithPagination productsDtoWithPagination = null;

                productsDtoWithPagination = string.IsNullOrEmpty((categoryId)) ?
                    proxy.GetProductsWithPagination(pagination) :
                    proxy.GetProductsForCategoryWithPagination(new Guid(categoryId), pagination);

                if (string.IsNullOrEmpty(categoryId))
                    ViewBag.CategoryName = "所有商品";
                else
                {
                    var category = proxy.GetCategoryById(new Guid(categoryId));
                    ViewBag.CategoryName = category.Name;
                }

                ViewBag.CategoryId = categoryId;
                ViewBag.FromIndexPage = fromIndexPage;
                if (fromIndexPage == null || fromIndexPage.Value)
                    ViewBag.Action = "Index";
                else
                    ViewBag.Action = "Category";
                ViewBag.IsFirstPage = productsDtoWithPagination.Pagination.PageNumber == 1;
                ViewBag.IsLastPage = productsDtoWithPagination.Pagination.PageNumber == productsDtoWithPagination.Pagination.TotalPages;
                return PartialView(productsDtoWithPagination);
            }
        }
        #endregion 

    }
}
