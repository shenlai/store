using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Domain.Repositories;
using Store.Domain.Model;
using Store.Repositories.EntityFramework;
using System.Linq.Expressions;
using Store.Domain.Specifications;

namespace Store.Repositories.EntityFramework
{
    public class UserRepository :EntityFrameworkRepository<User>, IUserRepository
    {

        //注： base(context) 执行前调用基类的构造函数
        public UserRepository(IRepositoryContext context)
            : base(context)
        { }

        #region public Methods

        public bool CheckPassword(string userName, string password)
        {
            Expression<Func<User, bool>> userNameExpression = u => u.UserName == userName;
            Expression<Func<User, bool>> passwordExpression = p => p.Password == password;

            //[1] 使用规约
            GetBySpecification(new ExpressionSpecification<User>(userNameExpression.And(passwordExpression)));

            //[2] 直接表达式重组
            GetByExpression(userNameExpression.And(passwordExpression));

            return base.Exists(new ExpressionSpecification<User>(userNameExpression.And(passwordExpression)));
        }

        #endregion




    }
}
