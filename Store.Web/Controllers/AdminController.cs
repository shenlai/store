using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Store.Web.OrderService;
using Store.Web.ProductService;
using Store.Web.UserService;
using Store.Web.ViewModels;
using CategoryDto = Store.Web.ProductService.CategoryDto;
using ProductDto = Store.Web.ProductService.ProductDto;
namespace Store.Web.Controllers
{
    [HandleError]
    public class AdminController : BaseController
    {
        #region Common Utility Actions

        /// <summary>
        /// 保存图片到服务器制定目录下
        /// </summary>
        /// <param name="postedFile"></param>
        /// <param name="filePath"></param>
        /// <param name="saveName"></param>
        [NonAction]
        private void SaveFile(HttpPostedFileBase postedFile, string filePath, string saveName)
        {
            string phyPath = Request.MapPath("~" + filePath);
            if (!Directory.Exists(phyPath))
            {
                Directory.CreateDirectory(phyPath);
            }
            try
            {
                postedFile.SaveAs(phyPath + saveName);
            }
            catch (Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        //图片上传功能的实现
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase fileData, string folder)
        {
            var result = string.Empty;
            if (fileData != null)
            {
                string ext = Path.GetExtension(fileData.FileName);//获取图片扩展名
                result = Guid.NewGuid() + ext;
                SaveFile(fileData, Url.Content("~/Images/Products/"), result);
            }
            return Content(result);//返回新生成的图片名称，前端显示
        }

        #endregion

        #region Products Manager
        [Authorize]
        public ActionResult AddProduct()
        {
            using (var proxy = new ProductServiceClient())
            {
                var categories = proxy.GetCategories().ToList();
                var item = new CategoryDto()
                {
                    Id = Guid.Empty.ToString(),
                    Name = "未分类",
                    Description = "未分类"
                };

                //categories.ToList().Insert(0, new CategoryDto() { Id = Guid.Empty.ToString(), Name = "(未分类)", Description = "(未分类)" });
                categories.Insert(0, item);
                ViewData["categories"] = new SelectList(categories, "Id", "Name", Guid.Empty.ToString());
                List<int> x = new List<int>();
                x.Insert(0, 1);

                return View();
            }
        }
        [HttpPost]
        [Authorize]
        public ActionResult AddProduct(ProductDto product)
        {
            using (var proxy = new ProductServiceClient())
            {
                if (string.IsNullOrEmpty(product.ImageUrl))
                {
                    var fileName = Guid.NewGuid() + ".png";
                    System.IO.File.Copy(Server.MapPath("~/Images/Products/ProductImage.png"),
                        Server.MapPath(string.Format("~/Images/Products/{0}", fileName)));
                    product.ImageUrl = fileName;
                }
                var addedProducts = proxy.CreateProducts(new List<ProductDto> { product }.ToArray());
                if (product.Category != null &&
                    product.Category.Id != Guid.Empty.ToString())
                    proxy.CategorizeProduct(new Guid(addedProducts[0].Id), new Guid(product.Category.Id));
                return RedirectToSuccess("添加商品信息成功!", "Products", "Admin");
            }
        }

        #endregion

        #region Products
        [Authorize]
        public ActionResult Products()
        {/*不带分页, 前端grid.GetHtml自带分页功能*/
            using (var proxy = new ProductServiceClient())
            {
                var model = proxy.GetProducts();
                return View(model);
            }
        }
        #endregion

        #region Orders
        public ActionResult Orders()
        {
            using (var proxy = new OrderServiceClient())
            {
                var model = proxy.GetAllOrders();
                return View(model);
            }
        }

        public ActionResult Order(string id)
        {
            using (var proxy = new OrderServiceClient())
            {
                var model = proxy.GetOrder(new Guid(id));
                return View(model);
            }
        }

        /// <summary>
        /// 发货
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DispatchOrder(string id)
        {
            using (var proxy = new OrderServiceClient())
            {
                proxy.Dispatch(new Guid(id));
                return RedirectToSuccess(string.Format("订单 {0} 已成功发货！", id.ToUpper()), "Orders", "Admin");
            }
        }
        #endregion

    }
}