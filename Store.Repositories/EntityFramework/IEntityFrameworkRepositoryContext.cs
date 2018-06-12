using Store.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repositories.EntityFramework
{
    //注：IRepositoryContext:IUnitOfWork 其中commit 在 EntityFrameworkRepositoryContext中实现。
    public interface IEntityFrameworkRepositoryContext : IRepositoryContext
    {
        #region Properties
        StoreDbContext DbContext { get; }
        #endregion 
    }
}
