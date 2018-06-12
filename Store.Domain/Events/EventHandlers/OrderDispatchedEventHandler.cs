using Store.Domain.Model;
using Store.Domain.Repositories;
using Store.Events.Bus;

namespace Store.Domain.Events.EventHandlers
{
    /// <summary>
    /// 发货事件处理器
    /// 某个领域对象的事件：它是一个事件处理类，它实现了IEventHandler，它所处理的事情需要在Handle里去完成
    /// </summary>
    public class OrderDispatchedEventHandler:IDomainEventHandler<OrderDispatchedEvent>
    {
        private readonly IEventBus _bus;

        public OrderDispatchedEventHandler(IEventBus bus)
        {
            this._bus = bus;
        }

        /// <summary>
        /// 为什么加 @ ? 因为event是关键字
        /// </summary>
        /// <param name="event"></param>
        public void Handle(OrderDispatchedEvent @event)
        {
            //获得事件源对象
            var order = @event.Source as Order;
            //更新事件源对象的属性
            if (order == null)
                return;

            order.DispatchedDate = @event.DispatchedDate;
            order.Status = OrderStatus.Dispatched;

            //这里把领域事件认为是一种消息，推送到EventBus中进行进一步处理。
            this._bus.Publish<OrderDispatchedEvent>(@event);
 
        }
    }
}
