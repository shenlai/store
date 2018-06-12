using Store.Domain.Model;
using Store.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Repositories.EntityFramework
{
    public class OrderRepository:EntityFrameworkRepository<Order>,IOrderRepository
    {
        public OrderRepository(IRepositoryContext context)
            : base(context)
        { }
    }
}
