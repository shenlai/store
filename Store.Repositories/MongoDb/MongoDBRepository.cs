using Store.Domain;
using Store.Domain.Repositories;
using Store.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using Store.Domain.Enum;
using Store.Infrastructure;

namespace Store.Repositories.MongoDb
{
    public abstract class MongoDBRepository<TAggregateRoot>: IRepository<TAggregateRoot> 
        where TAggregateRoot : class, IAggregateRoot
    {
        #region private field
        private readonly IMongoDBRepositoryContext mongoDBRepositoryContext;

        #endregion

        #region Ctor
        protected MongoDBRepository(IRepositoryContext context)
        {
            if (context is IMongoDBRepositoryContext)
                this.mongoDBRepositoryContext = context as MongoDBRepositoryContext;
            else
                throw new InvalidOperationException("Invalid mongodbRepository context type ");
        }
        #endregion
        /*internal  只有在同一程序集的文件中，内部类型或成员才是可访问的 EF那边上下文用的protected */  
        internal IMongoDBRepositoryContext MongoDBRepositoryContext
        {
            get { return this.mongoDBRepositoryContext; }
        }

        #region Protected Methods

        public void Add(TAggregateRoot aggregateroot)
        {
            this.mongoDBRepositoryContext.RegisterNew(aggregateroot);
        }

        public void Update(TAggregateRoot aggregateRoot)
        {
            this.mongoDBRepositoryContext.RegisterModified(aggregateRoot);
        }

        public void Remove(TAggregateRoot aggregateRoot)
        {
            this.mongoDBRepositoryContext.RegisterDeleted(aggregateRoot);
        }

        public bool Exists(ISpecification<TAggregateRoot> specification)
        {
            var collection = this.mongoDBRepositoryContext.GetCollectionForType(typeof(TAggregateRoot));
            return collection.AsQueryable<TAggregateRoot>().Where(specification.Expression).FirstOrDefault() != null;
        }

        public IEnumerable<TAggregateRoot> GetAll()
        {
            var collection = this.mongoDBRepositoryContext.GetCollectionForType(typeof(TAggregateRoot));
            var query = collection.AsQueryable<TAggregateRoot>().ToList();
            return query;
        }

        // 根据聚合根的ID值，从仓储中读取聚合根
        public TAggregateRoot GetByKey(Guid key)
        {
            return null;
        }

        public TAggregateRoot GetBySpecification(ISpecification<TAggregateRoot> spec)
        {
            return null;
        }
        public TAggregateRoot GetByExpression(Expression<Func<TAggregateRoot, bool>> expression)
        {
            return null;
        }

        // 以指定的排序字段和排序方式，从仓储中读取所有聚合根。
        public IEnumerable<TAggregateRoot> GetAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
        {
            return null;
        }

        //  根据指定的规约获取聚合根
        public IEnumerable<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification)
        {
            return null;
        }

        // 根据指定的规约,以指定的排序字段和排序方式，从仓储中读取聚合根
        public IEnumerable<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder)
        {
            return null;
        }

        public PagedResult<TAggregateRoot> GetAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate,
            SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return null;
        }

        public PagedResult<TAggregateRoot> GetAll(
            ISpecification<TAggregateRoot> specification,
            Expression<Func<TAggregateRoot, dynamic>> sortPredicate,
            SortOrder sortOrder, int pageNumber, int pageSize)
        {
            return null;
        }

        public PagedResult<TAggregateRoot> GetAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate,
            SortOrder sortOrder, int pageNumber, int pageSize,
            params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return null;
        }

        public PagedResult<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification,
            Expression<Func<TAggregateRoot, dynamic>> sortPredicate,
            SortOrder sortOrder, int pageNumber, int pageSize,
            params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return null;
        }

        public TAggregateRoot GetBySpecification(ISpecification<TAggregateRoot> specification, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return null;
        }
        public IEnumerable<TAggregateRoot> GetAll(params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return null;
        }

        public IEnumerable<TAggregateRoot> GetAll(Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return null;
        }

        public IEnumerable<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return null;
        }

        public IEnumerable<TAggregateRoot> GetAll(ISpecification<TAggregateRoot> specification, Expression<Func<TAggregateRoot, dynamic>> sortPredicate, SortOrder sortOrder, params Expression<Func<TAggregateRoot, dynamic>>[] eagerLoadingProperties)
        {
            return null;
        }
        #endregion


    }
}
