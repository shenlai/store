using System;
using System.Collections.Generic;
using System.Linq;
using Store.Domain.Model;
using Store.Domain.Repositories;
using Store.Infrastructure;

namespace Store.Repositories.EntityFramework
{
    public class ProductCategorizationRepository : EntityFrameworkRepository<ProductCategorization>, IProductCategorizationRepository
    {
        public ProductCategorizationRepository(IRepositoryContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 获取指定分类下的所有商品信息
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetProductsForCategory(Category category)
        {
            var context = EfContext.DbContext;
            if (context == null)
                throw new InvalidOperationException("指定的仓储上下文（Repository Context）无效.");

            var query = from product in context.Products
                        from categorization in context.ProductCategorizations
                        where product.Id == categorization.ProductId &&
                              categorization.CategoryId == category.Id
                        select product;

            return query.ToList();
        }

        /// <summary>
        /// 以分页的方式，获取指定分类下的所有商品信息
        /// </summary>
        /// <param name="category">指定的商品分类</param>
        /// <param name="pageNumber">所请求的分页页码</param>
        /// <param name="pageSize">所请求的页大小</param>
        /// <returns>指定分类下的某页的商品信息</returns>
        public PagedResult<Product> GetProductsForCategoryWithPagination(Category category, int pageNumber, int pageSize)
        {
            var context = EfContext.DbContext;
            if (context == null)
                throw new InvalidOperationException("指定的仓储上下文（Repository Context）无效.");

            var skip = (pageNumber - 1) * pageSize;
            var take = pageSize;

            var query = from product in context.Products
                        from categorization in context.ProductCategorizations
                        where product.Id == categorization.ProductId &&
                              categorization.CategoryId == category.Id
                        orderby product.Name ascending
                        select product;

            var pagedQuery = query.Skip(skip).Take(take).GroupBy(p => new { Total = query.Count() }).FirstOrDefault();
            return pagedQuery == null ? null : new PagedResult<Product>(pagedQuery.Key.Total, (pagedQuery.Key.Total + pageSize - 1) / pageSize, pageSize, pageNumber, pagedQuery.Select(p => p).ToList());
        }

        /// <summary>
        /// 获取商品所属的商品分类。
        /// </summary>
        /// <param name="product">商品信息。</param>
        /// <returns>商品分类。</returns>
        public Category GetCategoryForProduct(Product product)
        {
            var context = EfContext.DbContext;
            if (context == null) throw new InvalidOperationException("指定的仓储上下文（Repository Context）无效。");

            var query = from category in context.Categories
                        from categorization in context.ProductCategorizations
                        where categorization.ProductId == product.Id &&
                              categorization.CategoryId == category.Id
                        select category;
            return query.FirstOrDefault();
        }
    }
}