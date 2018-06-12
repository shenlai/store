using Store.Domain.Model;
using Store.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repositories.EntityFramework
{
    public class ProductRepository : EntityFrameworkRepository<Product>,IProductRepository
    {

        #region Ctor

        /*
         * 'Store.Repositories.EntityFramework.EntityFrameworkRepository<Store.Domain.Model.Product>' 
         * does not contain a constructor that takes 0 arguments	
         * F:\Project\Store\Store.Repositories\EntityFramework\ProductRepository.cs	30	18	Store.Repositories
         */
        /*
         * 原因：类构造函数是层层向上寻找，直到基类，然后执行，
         * 然后一层层向下执行，此时我们来看EntityFrameworkRepository类中的构造函数EntityFrameworkRepository(IRepositoryContext context);
         * 如果向父类执行，此时却没有指定执行父类中的哪一个构造函数，默认情况下会去执行父类中无参数的构造函数
         * (备注：)一个类显式地声明了任何构造函数，编译器不生成公有的默认构造函数。这这种情况下，如果程序需要一个默认构造函数，需要由类的设计者提供。
         */
        //public ProductRepository(IRepositoryContext context)
        //{
        //    var efContext = context as IEntityFrameworkRepositoryContext;
        //    if (efContext != null)
        //        this._efContext = efContext;
        //}

        /*指定执行基类构造函数*/
        public ProductRepository(IRepositoryContext context)
            : base(context)
        { }
        #endregion

        public IEnumerable<Product> GetNewProducts(int count = 0)
        {
            var ctx = this.EfContext.DbContext as StoreDbContext;
            if (ctx == null)
                return null;
            var query = from p in ctx.Products
                        where p.IsNew == true
                        select p;
            if (count == 0)
                return query.ToList();
            else
                return query.Take(count).ToList();
        }

        #region 这些方法 已在EntityFrameworkRepository 中实现
        //public void Add(Product aggregateRoot)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<Product> GetAll()
        //{
        //    var ctx = this.EfContext.DbContext as StoreDbContext;
        //    if (ctx == null)
        //        return null;
        //    var query = from p in ctx.Products
        //                select p;
        //    return query.ToList();
        //}

        //public Product GetByKey(Guid key)
        //{
        //    return EfContext.DbContext.Products.First(p => p.Id == key);
        //}
        #endregion

    }
}
