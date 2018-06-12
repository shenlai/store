using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Model
{
    public enum OrderStatus
    {
        Created = 0, // 订单已被创建
        Paid, // 订单已付款
        Picked, // 订单已仓库拣货
        Dispatched, // 已发货
        Delevered // 已派送
    }
}
