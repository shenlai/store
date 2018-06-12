using System;
using Store.Domain.Events;
using Store.Infrastructure;

namespace Store.Events.Handlers
{

    [HandlesAsynchronously] //如果事件处理器添加了该属性，表示以异步的方式处理事件
    public class SendEmailHandler : IEventHandler<OrderDispatchedEvent>
    {
        public void Handle(OrderDispatchedEvent @event)
        {
            try
            {
                Utils.SendEmail(@event.UserEmailAddress,
                    "您的订单已经发货",
                    string.Format("您的订单{0}已于{1}发货,欢迎您随时关注订单状态",
                    @event.OrderId.ToString().ToUpper(),
                    @event.DispatchedDate)
                    );
            }
            catch (Exception ex)
            {
                //Utils.Log(ex);
            }
        }
    }
}
