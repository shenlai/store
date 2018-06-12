using Store.Domain.Model;
using Store.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;

namespace Store.Repositories.MongoDb
{

    /*sql server 数据导出到 MongoDB http://www.cnblogs.com/yuluhuang/p/4049029.html*/
    public class ProductRepository : MongoDBRepository<Product>,IProductRepository
    {
        #region Ctor
        public ProductRepository(IRepositoryContext context) : base(context)
        {
        }

        #endregion

        #region IProductRepository
        public IEnumerable<Product> GetNewProducts(int count = 0)
        {
            var productColl = this.MongoDBRepositoryContext.GetCollectionForType(typeof(Product));
            List<Product> products = null;
            if (count == 0)
            {
                products = productColl.AsQueryable<Product>().Where(p => p.IsNew == true).ToList();
            }
            else
            {
                products = products.AsQueryable<Product>().Where(p => p.IsNew == true).Take(count).ToList();
            }
            return products;
        }
        #endregion
    }
}
