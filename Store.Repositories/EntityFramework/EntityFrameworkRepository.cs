﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Domain.Repositories;
using Store.Domain;
using Store.Domain.Specifications;
using System.Linq.Expressions;
using Store.Domain.Enum;
using Store.Infrastructure;

namespace Store.Repositories.EntityFramework
{
    // 定义一个抽象类，实现代码的复用，因为不需要让每个具体的仓储类都去实现一遍Add,GetAll, Remove,Update逻辑
    // 所以定义一个抽象类来实现这些公共的逻辑，具体仓储类只需要另外实现具体逻辑
    public abstract class EntityFrameworkRepository<TAggregateRoot> : IRepository<TAggregateRoot>
        where TAggregateRoot : class,IAggregateRoot
    {
        //private  IEntityFrameworkRepositoryContext _efContext;
        private readonly IEntityFrameworkRepositoryContext _efContext;

        protected EntityFrameworkRepository(IRepositoryContext context)
        {
            var efContext = context as IEntityFrameworkRepositoryContext; // IEntityFrameworkRepositoryContext:IRepositoryContext
            if (efContext != null)
                this._efContext = efContext;
        }
        /*此方法作用？？*/
        //MemberExpression 表示访问字段或属性
        private MemberExpression GetMemberInfo(LambdaExpression lambda)
        {
            if (lambda == null)
                throw new ArgumentNullException("lambda");
            MemberExpression memberExpr = null;
            //lambda.Body lambda表达式的主体
            switch (lambda.Body.NodeType)
            {
                case ExpressionType.Convert:
                    memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
                    break;
                case ExpressionType.MemberAccess:
                    memberExpr = lambda.Body as MemberExpression;
                    break;
            }

            if (memberExpr == null)
                throw new ArgumentException("method");
            return memberExpr;
        }
        /*此方法作用*/
        private string GetEagerLoadingPath(Expression<Func<TAggregateRoot, dynamic>> eagerLoadingProperty)
        {
            var memberExpression = this.GetMemberInfo(eagerLoadingProperty);
            var parameterName = eagerLoadingProperty.Parameters.First().Name;
            var memberExpressionStr = memberExpression.ToString();
            var path = memberExpressionStr.Replace(parameterName + ".", "");
            return path;
        }
        protected IEntityFrameworkRepositoryContext EfContext
        {
            get { return this._efContext; }
        }

        #region 常用方法

        public bool Exists(ISpecification<TAggregateRoot> specification)
        {
            var count = EfContext.DbContext.Set<TAggregateRoot>().Count(specification.IsSatisfiedBy);
            return count != 0;
        }


        public void Add(TAggregateRoot aggregateroot)
        {
            // 调用IEntityFrameworkRepositoryContext的RegisterNew方法将实体添加进DbContext.DbSet对象中
            this._efContext.RegisterNew(aggregateroot);
        }

        /// <summary>
        /// 根据聚合根的ID值，从仓储中读取聚合根 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TAggregateRoot GetByKey(Guid key)
        {
            return this._efContext.DbContext.Set<TAggregateRoot>().First(x => x.Id == key);
        }

        public TAggregateRoot GetBySpecification(ISpecification<TAggregateRoot> spec)
        {
            return this._efContext.DbContext.Set<TAggregateRoot>().FirstOrDefault(spec.Expression);
        }

        public TAggregateRoot GetByExpression(Expression<Func<TAggregateRoot, bool>> expression)
        {
            var list = _efContext.DbContext.Set<TAggregateRoot>().ToList();
            return _efContext.DbContext.Set<TAggregateRoot>().FirstOrDefault(expression);
        }

        /// <summary>
        /// 获取（首先获取类别会用）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TAggregateRoot> GetAll()
        {
            return this._efContext.DbContext.Set<TAggregateRoot>();
            //return GetAll(new AnySpecification<TAggregateRoot>(), null, SortOrder.UnSpecified);
        }


        public IEnumerable<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification)
        {
            return GetAll(specification, null, SortOrder.UnSpecified);
        }

        public IEnumerable<TAggregateRoot> GetAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
        {
            return GetAll(new AnySpecification<TAggregateRoot>(), sortPredicate, sortOrder);
        }
        public IEnumerable<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
        {
            var query = _efContext.DbContext.Set<TAggregateRoot>().Where(specification.Expression);

            if (sortPredicate != null)
            {
                switch (sortOrder)
                {
                    case SortOrder.Ascending:
                        return query.SortBy(sortPredicate).ToList(); break;
                    case SortOrder.Descending:
                        return query.SortByDescending(sortPredicate).ToList();break;
                    default:
                        break;
                }
            }
            return query.ToList();
        }


        public void Remove(TAggregateRoot aggregateRoot)
        {
            this._efContext.RegisterDeleted(aggregateRoot);
        }

        public void Update(TAggregateRoot aggregateRoot)
        {
            this._efContext.RegisterModified(aggregateRoot);
        }

        #endregion

        #region 饥饿加载方式实现

        public TAggregateRoot GetBySpecification(ISpecification<TAggregateRoot> specification, 
            params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            var dbset = _efContext.DbContext.Set<TAggregateRoot>();
            if (eagerLoadingProperties != null && eagerLoadingProperties.Length > 0)
            {
                var eagerLoadingProperty = eagerLoadingProperties[0];
                var eagerLoadingPath = this.GetEagerLoadingPath(eagerLoadingProperty);
                var dbquery = dbset.Include(eagerLoadingPath);
                for (var i = 1; i < eagerLoadingProperties.Length; i++)
                {
                    eagerLoadingProperty = eagerLoadingProperties[i];
                    eagerLoadingPath = this.GetEagerLoadingPath(eagerLoadingProperty);
                    dbquery = dbquery.Include(eagerLoadingPath);
                }
                return dbquery.Where(specification.Expression).FirstOrDefault(); //dbquery
            }
            else
                return dbset.Where(specification.Expression).FirstOrDefault();   //dbset
        }

        public IEnumerable<TAggregateRoot> GetAll(params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return GetAll(new AnySpecification<TAggregateRoot>(), null, SortOrder.UnSpecified, eagerLoadingProperties);
        }

        public IEnumerable<TAggregateRoot> GetAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return GetAll(new AnySpecification<TAggregateRoot>(), sortPredicate, sortOrder, eagerLoadingProperties);
        }

        public IEnumerable<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return GetAll(specification, null, SortOrder.UnSpecified, eagerLoadingProperties);
        }

        public IEnumerable<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            var dbset = _efContext.DbContext.Set<TAggregateRoot>();
            IQueryable<TAggregateRoot> queryable = null;
            if (eagerLoadingProperties != null &&
                eagerLoadingProperties.Length > 0)
            {
                var eagerLoadingProperty = eagerLoadingProperties[0];
                var eagerLoadingPath = this.GetEagerLoadingPath(eagerLoadingProperty);
                var dbquery = dbset.Include(eagerLoadingPath);
                for (var i = 1; i < eagerLoadingProperties.Length; i++)
                {
                    eagerLoadingProperty = eagerLoadingProperties[i];
                    eagerLoadingPath = this.GetEagerLoadingPath(eagerLoadingProperty);
                    dbquery = dbquery.Include(eagerLoadingPath);
                }
                queryable = dbquery.Where(specification.Expression);
            }
            else
                queryable = dbset.Where(specification.Expression);

            if (sortPredicate != null)
            {
                switch (sortOrder)
                {
                    case SortOrder.Ascending:
                        return queryable.SortBy(sortPredicate).ToList();
                    case SortOrder.Descending:
                        return queryable.SortByDescending(sortPredicate).ToList();
                    default:
                        break;
                }
            }
            return queryable.ToList();
        }

        #endregion

        #region 分页支持
        public PagedResult<TAggregateRoot> GetAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return GetAll(new AnySpecification<TAggregateRoot>(), sortPredicate, sortOrder, pageNumber, pageSize);
        }

        // 分页也就是每次只取出每页展示的数据大小
        public PagedResult<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "页码必须大于等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            var query = _efContext.DbContext.Set<TAggregateRoot>()
                .Where(specification.Expression);
            var skip = (pageNumber - 1) * pageSize;
            var take = pageSize;
            if (sortPredicate == null)
                throw new InvalidOperationException("基于分页功能的查询必须指定排序字段和排序顺序。");

            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    var count = query.Count();
                    var sortby = query.SortBy(sortPredicate);
                    var skipp = query.SortBy(sortPredicate).Skip(skip);
                    var takk = query.SortBy(sortPredicate).Skip(skip).Take(take);
                    var test = query.SortBy(sortPredicate).Skip(skip).Take(take).GroupBy(p => new { Total = query.Count() });

                    var pagedGroupAscending = query.SortBy(sortPredicate).Skip(skip).Take(take).GroupBy(p=> new {Total = query.Count()}).FirstOrDefault();

                    if (pagedGroupAscending == null)
                        return null;
                    return new PagedResult<TAggregateRoot>(pagedGroupAscending.Key.Total, (pagedGroupAscending.Key.Total + pageSize - 1) / pageSize, pageSize, pageNumber, pagedGroupAscending.Select(p => p).ToList());
                case SortOrder.Descending:
                    var pagedGroupDescending = query.SortByDescending(sortPredicate).Skip(skip).Take(take).GroupBy(p => new { Total = query.Count() }).FirstOrDefault();
                    if (pagedGroupDescending == null)
                        return null;
                    return new PagedResult<TAggregateRoot>(pagedGroupDescending.Key.Total, (pagedGroupDescending.Key.Total + pageSize - 1) / pageSize, pageSize, pageNumber, pagedGroupDescending.Select(p => p).ToList());
                default:
                    break;
            }

            throw new InvalidOperationException("基于分页功能的查询必须指定排序字段和排序顺序。");
        }

        public PagedResult<TAggregateRoot> GetAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return GetAll(new AnySpecification<TAggregateRoot>(), sortPredicate, sortOrder, pageNumber, pageSize, eagerLoadingProperties);
        }

        public PagedResult<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, int pageNumber, int pageSize, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException("pageNumber", pageNumber, "页码必须大于等于1");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, "每页大小必须大于或等于1");

            // 将需要饥饿加载的内容添加到Include方法参数中
            var dbset = _efContext.DbContext.Set<TAggregateRoot>();

            IQueryable<TAggregateRoot> query = null;
            if (eagerLoadingProperties != null &&
                eagerLoadingProperties.Length > 0)
            {
                var eagerLoadingProperty = eagerLoadingProperties[0];
                var eagerLoadingPath = this.GetEagerLoadingPath(eagerLoadingProperty);
                var dbquery = dbset.Include(eagerLoadingPath);
                for (var i = 1; i < eagerLoadingProperties.Length; i++)
                {
                    eagerLoadingProperty = eagerLoadingProperties[i];
                    eagerLoadingPath = this.GetEagerLoadingPath(eagerLoadingProperty);
                    dbquery = dbquery.Include(eagerLoadingPath);
                }

                query = dbquery.Where(specification.Expression);
            }
            else
                query = dbset.Where(specification.Expression);

            var skip = (pageNumber - 1) * pageSize;
            var take = pageSize;

            if (sortPredicate == null)
                throw new InvalidOperationException("基于分页功能的查询必须指定排序字段和排序顺序。");

            switch (sortOrder)
            {
                case SortOrder.Ascending:
                    var pagedGroupAscending = query.SortBy(sortPredicate).Skip(skip).Take(take).GroupBy(p => new { Total = query.Count() }).FirstOrDefault();

                    if (pagedGroupAscending == null)
                        return null;
                    return new PagedResult<TAggregateRoot>(pagedGroupAscending.Key.Total, (pagedGroupAscending.Key.Total + pageSize - 1) / pageSize, pageSize, pageNumber, pagedGroupAscending.Select(p => p).ToList());
                case SortOrder.Descending:
                    var pagedGroupDescending = query.SortByDescending(sortPredicate).Skip(skip).Take(take).GroupBy(p => new { Total = query.Count() }).FirstOrDefault();
                    if (pagedGroupDescending == null)
                        return null;
                    return new PagedResult<TAggregateRoot>(pagedGroupDescending.Key.Total, (pagedGroupDescending.Key.Total + pageSize - 1) / pageSize, pageSize, pageNumber, pagedGroupDescending.Select(p => p).ToList());
                default:
                    break;
            }

            throw new InvalidOperationException("基于分页功能的查询必须指定排序字段和排序顺序。");
        }

        #endregion
    }
}
