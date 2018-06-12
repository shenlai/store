using System;

namespace Store.Events
{
    /// <summary>
    /// 领域对象事件标示：（标示接口，接口的一种，用来约束一批对象）
    /// </summary>
    public interface IEvent
    {
        Guid Id { get; }

        //获取事件产生的时间
        DateTime TimeStamp { get; }
    }
}
