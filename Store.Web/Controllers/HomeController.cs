using Store.Web.OrderService;
using Store.Web.ProductService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Store.Web.Controllers
{
    public class HomeController : BaseController
    {
        #region Protected Properties

        protected Guid UserId
        {
            get
            {
                if (Session["UserId"] != null)
                {
                    return (Guid)Session["UserId"];
                }
                else
                {
                    var id = new Guid(Membership.GetUser().ProviderUserKey.ToString());
                    Session["UserId"] = id;
                    return id;
                }
            }
        }

        #endregion

        public ActionResult Index()
        {
            using (var proxy = new ProductServiceClient())
            {
                var x = proxy.GetProducts();
                var re = proxy.sum(10);
                ViewBag.res = re;
            }


            return View();
        }

        [Authorize]
        public ActionResult AddToCart(string productId, string items)
        {
            using (var proxy = new OrderServiceClient())
            {
                int quantity = 0;
                if (!int.TryParse(items, out quantity))
                    quantity = 1;
                proxy.AddProductToCart(UserId, new Guid(productId), quantity);
                return RedirectToAction("ShoppingCart");
            }
        }

        public ActionResult ShoppingCart()
        {
            using (var proxy = new OrderServiceClient())
            {
                var model = proxy.GetShoppingCart(UserId);
                return View(model);
            }
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ProductDetail(string id)
        {
            using (var proxy = new ProductServiceClient())
            {
                var productModel = proxy.GetProductById(new Guid(id));
                return View(productModel);
            }
        }

        /// <summary>
        /// 结算操作
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult CheckOut()
        {
            using (var proxy = new OrderServiceClient())
            {
                var model = proxy.Checkout(this.UserId);
                return View(model);
            }
        }

        [Authorize]
        public ActionResult Orders()
        {
            using (var proxy = new OrderServiceClient())
            {
                var model = proxy.GetOrdersForUser(this.UserId);
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

        public ActionResult SuccessPage(string pageTitle, string pageMessage = null, string retAction = "Index", string retController = "Home", int waitSeconds = 5)
        {
            ViewBag.PageTitle = pageTitle;
            ViewBag.PageMessage = pageMessage;
            ViewBag.RetAction = retAction;
            ViewBag.RetController = retController;
            ViewBag.WaitSeconds = waitSeconds;
            return View();
        }
    }
}