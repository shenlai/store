using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Domain.Repositories;
using Store.Domain.Model;

namespace Store.Repositories.EntityFramework
{
    public class CategoryRepository :EntityFrameworkRepository<Category>, ICategoryRepository
    {
        #region Public Properties

        //public CategoryRepository(IRepositoryContext context)
        //{
        //    var efContext = context as IEntityFrameworkRepositoryContext;
        //    if (efContext != null)
        //        this._efContext = efContext;
        //}

        public CategoryRepository(IRepositoryContext context)
            : base(context)
        { }
        #endregion

        #region Public Method 父类已经实现

        //public void Add(Category aggregateRoot)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public IEnumerable<Category> GetAll()
        //{
        //    var ctx = this.EfContext.DbContext as StoreDbContext;
        //    if (ctx == null)
        //        return null;
        //    var query = from c in ctx.Categories
        //                select c;
        //    return query.ToList();
        //}

        //public Category GetByKey(Guid key)
        //{
        //    return this.EfContext.DbContext.Categories.First(c => c.Id == key);
        //}

        #endregion
    }
}