using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Store.Infrastructure
{
    public class CallContext
    {
        [ThreadStatic]
        public static bool Disabled = false;

        /// <summary>
        /// 设置上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        public static void Set<T>(T context) where T : class
        {
            if (!Disabled)
            {
                CallContext<T>.Instance = context;
            }              
        }

        /// <summary>
        /// 取上下文
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="returnCloneInstance"></param>
        /// <returns></returns>
        public static T Get<T>(bool returnCloneInstance = false) where T : class
        {
            if (Disabled)
                //return default(T);
                return null;
            if (CallContext<T>.Instance != null)
            {
                if (returnCloneInstance)
                {
                    return ObjectHelper.DeepCopy<T, T>(CallContext<T>.Instance);
                }
                else
                {
                    return CallContext<T>.Instance;
                }
            }
            else
            {
                //return default(T);
                return null;
            }
        }


        public static bool Exist<T>() where T : class
        {
            if (Disabled)
                return false;
            return CallContext<T>.Instance != null;
        }

        public static void Remove<T>() where T : class
        {
            if (!Disabled)
            {
                CallContext<T>.Instance = null;
            }
        }




    }


    internal class CallContext<T> where T:class
    {
        [ThreadStatic]
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    //webform
                    var itemKey = "Current" + typeof(T).Name;
                    var item = HttpContext.Current.Items[itemKey];
                    if (item == null)
                        return item as T;
                    else
                        return default(T);
                }
                else if (WcfContext.Current != null)
                {
                    //wcf
                    var itemKey = "Current" + typeof(T).Name;
                    if (!WcfContext.Current.Items.ContainsKey(itemKey))
                        return default(T);

                    var item = WcfContext.Current.Items[itemKey];
                    if (item != null)
                        return item as T;
                    else
                        return default(T);
                }
                else
                {
                    //winform
                    return _instance;
                }
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    var itemKey = "Current" + typeof(T).Name;
                    HttpContext.Current.Items[itemKey] = value;
                }
                else if (WcfContext.Current != null)
                {
                    var itemKey = "Current" + typeof(T).Name;
                    WcfContext.Current.Items[itemKey] = value;
                }
                else
                {
                    _instance = value;
                }
            }
        }
    }


    public class WcfContext : IExtension<OperationContext>
    {
        private readonly IDictionary<string, object> items;
        private WcfContext()
        {
            items = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Items
        {
            get { return items; }
        }

        public static WcfContext Current
        {
            get
            {
                if (OperationContext.Current == null)
                {
                    return null;
                }
                var context = OperationContext.Current.Extensions.Find<WcfContext>();
                if (context == null)
                {
                    context = new WcfContext();
                    OperationContext.Current.Extensions.Add(context);
                }
                return context;

            }
        }

        public void Attach(OperationContext owner) { }

        public void Detach(OperationContext owner) { }




    }


}
