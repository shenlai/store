using Store.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Repositories
{
    public interface IProductRepository:IRepository<Product>
    {
        IEnumerable<Product> GetNewProducts(int count = 0);
    }
}
