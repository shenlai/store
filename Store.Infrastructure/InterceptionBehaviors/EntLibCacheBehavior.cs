using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Store.Infrastructure.Caching;

namespace Store.Infrastructure.InterceptionBehaviors
{
    public class EntLibCacheBehavior : IInterceptionBehavior
    {
        private readonly ICacheProvider _cacheProvider;
        public EntLibCacheBehavior()
        {
            _cacheProvider = ServiceLocator.Instance.GetService<ICacheProvider>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public string GetValueKey(EntLibCacheAttribute attribute,IMethodInvocation input)
        {
            switch(attribute.CacheMethod)
            {
                case CachingMethod.Get:
                case CachingMethod.Update:
                    if (input != null && input.Arguments != null)
                    {
                        var sb = new StringBuilder();
                        for (int i = 0; i < input.Arguments.Count; i++)
                        {
                            sb.Append(input.Arguments[i]);
                            if (i != input.Arguments.Count - 1)
                                sb.Append("_");
                        }
                        return sb.ToString();
                    }
                    else
                        return "null";
                default:
                    throw new InvalidOperationException("无效缓存方式");
            }

        }

        #region IInterceptionBehavior Members

        public bool WillExecute { get { return true; } }
      
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            var method = input.MethodBase;
            var key = method.Name;
            //1.未定义EntLibCache特性
            if (!method.IsDefined(typeof(EntLibCacheAttribute), false))
                return getNext().Invoke(input, getNext);
            //2.定义
            var cacheAttribute = method.GetCustomAttributes(typeof(EntLibCacheAttribute), false)[0] as EntLibCacheAttribute;
            var valueKey = GetValueKey(cacheAttribute, input);
            switch(cacheAttribute.CacheMethod)
            {
                case CachingMethod.Get:
                    try
                    {
                        // 如果缓存中存在该键值的缓存，则直接返回缓存中的结果退出
                        if (_cacheProvider.Exists(key,valueKey))
                        {
                            var value = _cacheProvider.Get(key, valueKey);
                            var arguments = new object[input.Arguments.Count];
                            input.Arguments.CopyTo(arguments, 0);
                            return new VirtualMethodReturn(input, value, arguments);

                            //input.CreateMethodReturn(value);
                        }
                        else // 否则先调用方法，再把返回结果进行缓存
                        {
                            var methodReturn = getNext().Invoke(input, getNext);
                            _cacheProvider.Add(key, valueKey, methodReturn.ReturnValue);
                            return methodReturn;
                        }
                            
                    }
                    catch(Exception e)
                    {
                        return new VirtualMethodReturn(input, e);
                    }
                default: break;

            }
            return getNext().Invoke(input, getNext);
        }


        #endregion




    }
}
