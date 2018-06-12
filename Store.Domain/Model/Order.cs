using Store.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Model
{
    public class Order : AggregateRoot
    {
        private List<OrderItem> _orderItems = new List<OrderItem>();

        #region Public Properties

        public OrderStatus Status { get; set; }

        /// <summary>
        /// 获取或设置订单的创建日期
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// 获取或设置订单的发货日期
        /// </summary>
        public DateTime? DispatchedDate { get; set; }

        /// <summary>
        /// 获取或设置订单的发货日期
        /// </summary>
        public DateTime? DeliveredDate { get; set; }

        /// <summary>
        /// 获取或设置订单的派送日期   注意：(导航属性通常被定义为virtual，使他们能获得某些实体框架的功能，比如延迟加载的优势)
        /// 缺陷：多次与DB交互，性能降低
        /// EF对于集合类型的导航属性会延迟加载
        /// </summary>
        public virtual List<OrderItem> OrderItems
        {
            get
            {
                return _orderItems;
            }
            set
            {
                _orderItems = value;
            }
        }

        public virtual User User { get; set; }

        public Address DeliveryAddress
        {
            get
            {
                return User.DeliveryAddress;//【这种写法 如果关闭延迟加载，将会抛异常，但不一定有去捕捉】
            }
        }

        // 在严格的业务系统中，金额往往以Money模式实现。有关Money模式，请参见：http://martinfowler.com/eaaCatalog/money.html
        public decimal Subtotal
        {
            get
            {
                return this.OrderItems.Sum(p => p.ItemAmout);
            }
        }

        #endregion

        #region Ctor
        public Order()
        {
            Id = Guid.NewGuid();
            CreatedDate = DateTime.Now;
            Status = OrderStatus.Created;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// 当客户完成收货后，对销售订单经行确认。
        /// </summary>
        public void Confirm()
        {
            // 处理领域事件
            //DomainEvent.Handle<OrderConfirmedEvent>(new OrderConfirmedEvent(this) { ConfirmedDate = DateTime.Now, OrderId = this.Id, UserEmailAddress = this.User.Email });
        }

        /// <summary>
        /// 处理发货（邮件）。
        /// </summary>
        public void Dispatch()
        {
            // 处理领域事件
            DomainEvent.Handle<OrderDispatchedEvent>(
                new OrderDispatchedEvent(this)
                {
                    DispatchedDate = DateTime.Now,
                    OrderId = this.Id,
                    UserEmailAddress = this.User.Email
                }
                );
        }
        #endregion

    }
}
