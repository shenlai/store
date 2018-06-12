using Store.Events;

namespace Store.Domain.Events
{
    /// <summary>
    /// 领域事件处理器
    /// </summary>
    /// <typeparam name="TDomainEvent"></typeparam>
    public interface IDomainEventHandler<in TDomainEvent>:IEventHandler<TDomainEvent>
        where TDomainEvent:class,IDomainEvent
    {
    }
}
