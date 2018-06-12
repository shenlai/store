using System;
using System.Collections.Generic;
using Store.Infrastructure;

namespace Store.Events.Bus
{
    public interface IBus:IUnitOfWork,IDisposable
    {
        Guid Id { get; }
        void Publish<TMessage>(TMessage message)
            where TMessage:class,IEvent;
        void Publish<TMessage>(IEnumerable<TMessage> messages)
            where TMessage : class,IEvent;
        void Clear();
    }
}
