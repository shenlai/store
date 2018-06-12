using Store.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Store.Events.Bus
{
    /*事件总线：事件处理核心类，承载了事件的发布，订阅与取消订阅的逻辑，EventBus*/

    //领域事件处理器只是对事件对象的状态进行更新
    //后续的事件处理操作交给EventBus进行处理
    //本案例中EventBus主要处理的任务就是发送邮件通知
    //在EventBus中一般处理应用事件，而领域事件处理器一般处理领域事件
    public class EventBus : DisposableObject, IEventBus
    {
        private readonly Guid _id = Guid.NewGuid();
        /*当使用ThreadLocal维护变量时，ThreadLocal为每个使用该变量的线程提供独立的变量副本，所以每一个线程都可以独立地改变自己的副本，而不会影响其它线程所对应的副本*/
        private readonly ThreadLocal<Queue<object>> _messageQueue = new ThreadLocal<Queue<object>>(() => new Queue<object>());
        private readonly ThreadLocal<bool> _committed = new ThreadLocal<bool>(() => true);
        private readonly IEventAggregator _aggregator;
        private readonly MethodInfo _handleMethod;

        public EventBus(IEventAggregator aggregator)
        {
            this._aggregator = aggregator;

            //获得EventAggregator中的Handle方法
            _handleMethod = (from m in aggregator.GetType().GetMethods()
                             let parameters = m.GetParameters()
                             let methodName = m.Name
                             where methodName == "Handle" && parameters != null
                             && parameters.Length == 1
                             select m).First();

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _messageQueue.Dispose();
                _committed.Dispose();
            }
        }

        #region IBus Members

        public Guid Id
        {
            get { return _id; }
        }

        public void Publish<TMessage>(TMessage message)
            where TMessage : class,IEvent
        {
            _messageQueue.Value.Enqueue(message);//将对象添加到队列结尾
            _committed.Value = false;
        }

        public void Publish<TMessage>(IEnumerable<TMessage> messages)
            where TMessage:class,IEvent
        {
            foreach (var message in messages)
            {
                Publish(message);
            }
        }

        public void Clear()
        {
            _messageQueue.Value.Clear();
            _committed.Value = true;
        }


        #endregion

        #region IUnitOfWork Members

        /// <summary>
        /// 触发应用事件处理器对事件进行处理
        /// </summary>
        public void Commit()
        {
            while (_messageQueue.Value.Count > 0)
            {
                var evnt = _messageQueue.Value.Dequeue();
                var evntType = evnt.GetType();
                var method = _handleMethod.MakeGenericMethod(evntType);

                //调用应用事件处理器对应用事件进行处理
                method.Invoke(_aggregator, new object[] { evnt });//调用方法
            }
            _committed.Value = true;
        }

        public void Rollback()
        {
            Clear();
        }
        //2016-07-27增加（暂未使用）
        public bool Committed { get; protected set; }
        //  以上 2016-07-27增加（暂未使用）
        #endregion

    }
}
