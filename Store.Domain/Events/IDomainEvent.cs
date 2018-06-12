using Store.Events;

namespace Store.Domain.Events
{
    public interface IDomainEvent:IEvent
    {
        /// <summary>
        /// 获取产生领域事件的事件源对象
        /// </summary>
        IEntity Source { get; }
    }
}
