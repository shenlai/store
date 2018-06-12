using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Domain.Model;

namespace Store.Domain.Repositories
{
    public interface IUserRepository:IRepository<User>
    {
        bool CheckPassword(string userName, string password);
    }
}
