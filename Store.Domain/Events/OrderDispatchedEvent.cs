using System;

namespace Store.Domain.Events
{
    /// <summary>
    /// 某个领域对象：为了实现某个业务，而创建的实体类，它里面有事件所需要的数据，它继承了IEvent
    /// </summary>
    [Serializable]
    public class OrderDispatchedEvent : DomainEvent
    {
        #region Ctor
        public OrderDispatchedEvent() { }

        public OrderDispatchedEvent(IEntity source)
            : base(source)
        { }

        #endregion

        #region Public Properties

        /// <summary>
        /// 获取或设置订单发货日期。
        /// </summary>
        public DateTime DispatchedDate { get; set; }
        public string UserEmailAddress { get; set; }
        public Guid OrderId { get; set; }

        #endregion
    }
}
