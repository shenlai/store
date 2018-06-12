
namespace Store.Events
{
    /// <summary>
    /// 领域对象的处理方法，(事件处理器接口)
    /// </summary>
    /// <typeparam name="TEvent">继承IEvent对象的事件源对象</typeparam>
    public interface IEventHandler<in TEvent>
        where TEvent:IEvent
    {
        //处理给定的事件
        void Handle(TEvent @event);
    }
}
