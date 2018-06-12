using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Infrastructure;
using System.Reflection;

namespace Store.Events
{
    /// <summary>
    /// 事件聚合器通过Unity注入（应用）事件的处理器。
    /// 在EventAggregator类中定义_eventHandlers来保存所有（应用）事件的处理器，
    /// 在EventAggregator的构造函数中通过调用其Register方法把对应的事件处理器添加
    /// 到_eventHandlers字典中。然后在EventBus中的Commit方法中通过找到EventAggregator中的
    /// Handle方法来触发事件处理器来处理对应事件，即发出邮件通知。
    /// 这里事件聚合器起到映射的功能，映射应用事件到对应的事件处理器来处理
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly object _sync = new object();
        private readonly Dictionary<Type, List<object>> _eventHandleers = new Dictionary<Type, List<object>>();
        private readonly MethodInfo _registerEventHandlerMethod;

        public EventAggregator()
        {

            /*为什么不用this.Register 获取Register方法*/ 

            //通过反射获得EventAggregator的Register方法
            _registerEventHandlerMethod = (from p in this.GetType().GetMethods()
                                           let methodName = p.Name
                                           let parameters = p.GetParameters()
                                           where methodName == "Register" &&
                                           parameters != null &&
                                           parameters.Length == 1 &&
                                           parameters[0].ParameterType.GetGenericTypeDefinition()
                                           == typeof(IEventHandler<>)
                                           select p).First();
        }

        /// <summary>
        /// 由注入时调用，参数从配置文件中读取
        /// </summary>
        /// <param name="handlers"></param>
        public EventAggregator(object[] handlers)
            : this()
        {
            //遍历注册EventHandler来把配置文件中的EventHandler通过Register添加进_eventHandlers字典中
            foreach (var obj in handlers)
            {
                var type = obj.GetType();
                var implementedInterfaces = type.GetInterfaces();
                foreach (var implementedInterface in implementedInterfaces)
                {
                    if (implementedInterface.IsGenericType && //是否泛型类型
                        implementedInterface.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                    {
                        var eventType = implementedInterface.GetGenericArguments().First();
                        //泛型方法Register<TEvent>
                        var method = _registerEventHandlerMethod.MakeGenericMethod(eventType);

                        //调用Register方法将EventHandler添加进_eventHandlers字典中
                        method.Invoke(this, new object[] { obj });
                    }
                }
            }
        }

        public void Register<TEvent>(IEventHandler<TEvent> eventHandler)
            where TEvent : class,IEvent
        {
            lock (_sync)
            {
                var eventType = typeof(TEvent);
                if (_eventHandleers.ContainsKey(eventType))
                {
                    var handlers = _eventHandleers[eventType];
                    if (handlers != null)
                    {
                        handlers.Add(eventHandler);
                    }
                    else
                    {
                        handlers = new List<object> { eventHandler };
                    }
                }
                else
                {
                    _eventHandleers.Add(eventType, new List<object> { eventHandler });
                }
            }
        }

        public void Register<TEvent>(IEnumerable<IEventHandler<TEvent>> eventHandlers)
            where TEvent : class,IEvent
        {
            foreach (var eventHandler in eventHandlers)
            {
                Register<TEvent>(eventHandlers);
            }
        }

        /// <summary>
        /// 调用具体的EventHandler的Handle方法来对事件进行处理
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evnet"></param>
        public void Handle<TEvent>(TEvent evnt)
            where TEvent : class,IEvent
        {
            if (evnt == null)
                throw new ArgumentNullException("evnt");
            var eventType = evnt.GetType();
            if (_eventHandleers.ContainsKey(eventType) &&
                _eventHandleers[eventType] != null &&
                _eventHandleers[eventType].Count > 0)
            {
                var handlers = _eventHandleers[eventType];
                foreach (var handler in handlers)
                {
                    var eventHandler = handler as IEventHandler<TEvent>;
                    if (eventHandler == null)
                        continue;

                    //异步处理
                    if (eventHandler.GetType().IsDefined(typeof(HandlesAsynchronouslyAttribute), false))
                    {
                        Task.Factory.StartNew(o => eventHandler.Handle((TEvent)o), evnt);
                        //Task.Factory.StartNew((o) => eventHandler.Handle((TEvent)o), evnt);
                    }
                    else
                    {
                        eventHandler.Handle(evnt);
                    }
                }
            }
        }

   }
}
