using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Infrastructure;
using System.Messaging;
using System.Reflection;

namespace Store.Events.Bus
{
    /// <summary>
    /// 基于微软MSMQ的EventBus的实现
    /// </summary>
    public class MsmqEventBus:DisposableObject,IEventBus
    {
        // private   -   这个成员只能在本类内使用  
        // static   -   这个成员不需要实例化即可使用  
        // readonly   -   这个成员只能在“类初始化”时赋值
        /*
         * 感觉上好像跟   const   有点联系，但是   const   是在编译的时候就计算结果的，这样的话，多数引用类型都无法赋值——new   操作符只能在运行时使用。我所知道的可以写到   const   字段的引用类型就只有字符串一种。static   readonly   的话，在第一次访问该类的时候才赋值，所以能用new，如上述例子  
            最后，const隐含static的语义，所以只需要写private   const即可 
         */
        #region Private Fields
        private readonly Guid _id = Guid.NewGuid();
        /*volatile多用于多线程的环境，当一个变量定义为volatile时，读取这个变量的值时候每次都是从momery里面读取而不是从cache读。这样做是为了保证读取该变量的信息都是最新的，而无论其他线程如何更新这个变量。*/
        private volatile bool _committed = true;
        private readonly bool _userInternalTranscation;
        private readonly MessageQueue _messageQueue;
        private readonly IEventAggregator _aggregator;
        private readonly MethodInfo _publishMethod;

        #endregion

        #region Ctor

        /// <summary>
        /// unity注入会调用此构造函数，path由配置文件中读取
        /// </summary>
        /// <param name="path"></param>
        public MsmqEventBus(string path)
        {
            this._aggregator = ServiceLocator.Instance.GetService<IEventAggregator>();
            _publishMethod = (from m in this._aggregator.GetType().GetMethods()
                              let parameters = m.GetParameters()
                              let methodName = m.Name
                              where methodName == "Handle" &&
                              parameters != null &&
                              parameters.Length == 1
                              select m
                                  ).First();
            var options = new MsmqBusOptions(path);

            //初始化消息队列对象
            #region 参数解释
            /*
             * 参数
               path
    类型：System.String
    此 MessageQueue 引用的队列的位置，它对于本地计算机可以是“.”。

sharedModeDenyReceive
    类型：System.Boolean
    true ，授予访问该队列的第一个应用程序独占读访问权；否则为 false。

enableCache
    类型：System.Boolean
    如果创建和使用连接缓存，则为 true；否则为 false。

accessMode
    类型：System.Messaging.QueueAccessMode
    QueueAccessMode 值之一。
             */
            #endregion
            this._messageQueue = new MessageQueue(path, options.SharedModeDenyReceive, options.EnableCache, options.QueueAccessMode);
            this._userInternalTranscation = options.UserInternalTransaction && _messageQueue.Transactional;
        }

        #endregion

        #region IEventBus Members

        public Guid Id
        {
            get { return this._id; }
        }

        /// <summary>
        /// 将消息内容放入message中body属性，并进行序列化发送到消息队列
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        public void Publish<TMessage>(TMessage message) where TMessage:class,IEvent
        {
            //将消息内容放入message中body属性，并进行序列化发送到消息队列
            var msg = new Message(message) { Formatter = new XmlMessageFormatter(new Type[] { typeof(TMessage) }), Label = message.GetType().ToString() };
            _messageQueue.Send(msg);
            _committed = false;
        }

        public void Publish<TMessage>(IEnumerable<TMessage> messages)
            where TMessage : class,IEvent
        {
            messages.ToList().ForEach(m =>
            {
                _messageQueue.Send(m);
                _committed = false;
            });
        }


        public void Clear()
        {
            this._messageQueue.Close();
        }

        
        /// <summary>
        /// Commit方法即从系统的消息队列中出队来获得消息
        /// </summary>
        public void Commit()
        {
            //内部事务性队列
            if (this._userInternalTranscation)
            {
                using (var tran = new MessageQueueTransaction())
                {
                    try
                    {
                        tran.Begin();
                        var message = _messageQueue.Receive();//接受(如果队列为空将会出问题？？)
                        if (message != null)
                        {
                            message.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                            var eventType = ConvertStringToType(message.Body.ToString());
                            var method = _publishMethod.MakeGenericMethod(eventType);
                            var evnt = Activator.CreateInstance(eventType);
                            method.Invoke(_aggregator, new object[] { evnt });
                            tran.Commit();
                        }
                    }
                    catch
                    {
                        tran.Abort();
                        throw;
                    }
                }
            }
            else
            {
                //从msmq消息队里中出发，此时获得的对象是消息对象
                var message = _messageQueue.Receive();
                if (message != null)
                {
                    //指定反序列化的对象，由于我们之前把对应的事件类型保存在MessageQueue中的Label属性中
                    //所以此时可以根据Label属性来获得目标序列化类型
                    message.Formatter = new XmlMessageFormatter(new[] { ConvertStringToType(message.Label) });

                    //这样message.Body获得就是对应的事件对象，后面的处理逻辑就和EventBus一样
                    var enentType = message.Body.GetType();
                    var method = _publishMethod.MakeGenericMethod(enentType);
                    method.Invoke(_aggregator, new object[] { message.Body });
                }
            }

            _committed = true;
        }

        //2016-07-27增加（暂未使用）
        public bool Committed { get; protected set; }

        public void Rollback()
        { }
        //  以上 2016-07-27增加（暂未使用）
        #endregion

        #region DisposableObject Members
        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (this._messageQueue == null) return;
            _messageQueue.Close();
            _messageQueue.Dispose();
        }
        #endregion


        private Type ConvertStringToType(string sourceStr)
        {
            return Type.GetType(sourceStr + ",Store.Domain");
        }

        

    }


}
