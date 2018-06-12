using Store.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Repositories
{
    //仓储上下文接口
    /// <summary>
    /// 这里把传统的IUnitOfWork接口中方法分别在2个接口定义：一个是IUnitOfWork,另一个就是该接口
    /// </summary>
    public interface IRepositoryContext:IUnitOfWork
    {
        /// <summary>
        /// 用来表示仓储上下文
        /// </summary>
        Guid Id { get; }

        void RegisterNew<TAggregateRoot>(TAggregateRoot entity)
            where TAggregateRoot : class,IAggregateRoot;

        void RegisterModified<TAggregateRoot>(TAggregateRoot entity)
            where TAggregateRoot : class,IAggregateRoot;

        void RegisterDeleted<TAggregateRoot>(TAggregateRoot entity)
            where TAggregateRoot : class,IAggregateRoot;
    }
}
