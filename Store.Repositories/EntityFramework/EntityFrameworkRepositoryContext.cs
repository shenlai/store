using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Entity;

namespace Store.Repositories.EntityFramework
{
    /// <summary>
    /// 真正提供 数据库上下文 对象的类！！！
    /// 实现:IEntityFrameworkRepositoryContext 和 IRepositoryContext：IUnitOfWork。 
    /// </summary>
    public class EntityFrameworkRepositoryContext:IEntityFrameworkRepositoryContext
    {
        //// 引用定义的StoreDbContext类对象
        //public StoreDbContext DbContex
        //{
        //    get {return new StoreDbContext();}
        //}


        // ThreadLocal代表线程本地存储，主要相当于一个静态变量
        // 但静态变量在多线程访问时需要显式使用线程同步技术。
        // 使用ThreadLocal变量，每个线程都会一个拷贝，从而避免了线程同步带来的性能开销
        private readonly ThreadLocal<StoreDbContext> _localCtx = new ThreadLocal<StoreDbContext>(() => new StoreDbContext());

        public StoreDbContext DbContext
        {
            get { return _localCtx.Value; }
        }

        #region IRepositoryContext Members
        private readonly Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get { return _id; }
        }
                                                           
        public void RegisterNew<TAggregrateRoot>(TAggregrateRoot entity)
            where TAggregrateRoot : class,Domain.IAggregateRoot
        {
            _localCtx.Value.Set<TAggregrateRoot>().Add(entity);
        }

        public void RegisterModified<TAggregrateRoot>(TAggregrateRoot entity)
            where TAggregrateRoot : class,Domain.IAggregateRoot
        {
            //this._localCtx.Value.Set<TAggregrateRoot>().Attach(entity); 这一句要不要？？
            _localCtx.Value.Entry<TAggregrateRoot>(entity).State = EntityState.Modified;
        }

        public void RegisterDeleted<TAggregateRoot>(TAggregateRoot entity) where TAggregateRoot : class, Domain.IAggregateRoot
        {
            _localCtx.Value.Set<TAggregateRoot>().Remove(entity);
        }

        #endregion

        #region UnitOfWork Members
        
        public void Commit()
        {
            var validationError = this._localCtx.Value.GetValidationErrors();
            _localCtx.Value.SaveChanges();
        }

        //2016-07-27增加（暂未使用）
        public bool Committed { get; protected set; }

        public void Rollback()
        { }
        //  以上 2016-07-27增加（暂未使用）
        #endregion

    }
}
