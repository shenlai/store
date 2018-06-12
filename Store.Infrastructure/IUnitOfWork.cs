using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure
{
    /// <summary>
    /// /// <remarks>有关Unit Of Work的详细信息，请参见UnitOfWork模式：http://martinfowler.com/eaaCatalog/unitOfWork.html。
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// 提交当前Unit of Work
        /// </summary>
        void Commit();

        //2016-07-27 增加
        /// <summary>
        /// 表示当前Unit of Work事务是否已被提交
        /// </summary>
        bool Committed { get; }

        /// <summary>
        /// 回滚当前的Unit Of Work
        /// </summary>
        void Rollback();
        //以上 2016-07-27 增加
    }
}
