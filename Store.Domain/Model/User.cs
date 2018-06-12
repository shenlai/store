using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Model
{
    public class User : AggregateRoot
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime RegisteredDate { get; set; }

        public DateTime? LastLogonDate { get; set; }

        public string Contact { get; set; }

        //用户的联系地址
        //使用codeFirst 会将怎个Address属性作为字段生成到 User表中，是不是想要的结果？？？？？
        public Address ContactAddress { get; set; }

        //用户的发货地址
        public Address DeliveryAddress { get; set; }

        public IEnumerable<Order> Orders
        {
            get
            {
                IEnumerable<Order> result = null;
                //DomainEvent.Publish<GetUserOrdersEvent>(new GetUserOrdersEvent(this),
                //    (e, ret, exc) =>
                //    {
                //        result = e.Orders;
                //    });
                return result;
            }
        }

        public override string ToString()
        {
            return this.UserName;
        }

        #region Public Methods

        public void Disable()
        {
            this.IsDisabled = true;
        }

        public void Enable()
        {
            this.IsDisabled = false;
        }

        // 为当前用户创建购物篮。
        public ShoppingCart CreateShoppingCart()
        {
            var shoppingCart = new ShoppingCart { User = this };
            return shoppingCart;
        }
        #endregion
    }

}
