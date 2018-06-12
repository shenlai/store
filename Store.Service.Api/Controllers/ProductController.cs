using Store.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Mvc;

namespace Store.Service.Api.Controllers
{
    [RoutePrefix("api/Product")]
    public class ProductController : ApiController
    {
        private readonly IProductService productService;
        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        [Route("GetProducts")]
        public HttpResponseMessage GetProducts()
        {

            //var res = productService.sum(5);
            var res = productService.GetProducts();
            var resmessage = new HttpResponseMessage(HttpStatusCode.OK)
            {   
                Content = new StringContent(res.ToString(),System.Text.Encoding.UTF8,"application/json")
            };

            return resmessage;

        }

        /// <summary>
        /// 测试mongo实现的仓储
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProductsByMongoRep")]
        public HttpResponseMessage GetProductsByMongoRep()
        {
            var res = productService.TestGetProducts();
            var resmessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(res.ToString(), System.Text.Encoding.UTF8, "application/json")
            };

            return resmessage;

        }

















        #region
        //// GET: api/Product
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET: api/Product/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Product
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Product/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Product/5
        //public void Delete(int id)
        //{
        //}
        #endregion
    }
}
