using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure.Caching
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]//Inherited=false 不可继承
    public class EntLibCacheAttribute:Attribute
    {
        public CachingMethod CacheMethod { get; set; }

        public bool IsForce { get; set; }

        // 缓存相关的方法名称，该参数仅在Remove的方式用到
        public string[] CorrespondingMethodNames { get; set; }

        public EntLibCacheAttribute(CachingMethod method)
        {
            this.CacheMethod = method;
        }
        public EntLibCacheAttribute(CachingMethod method,params string[] methodNames):this(method)
        {
            this.CorrespondingMethodNames = methodNames;
        }
    }
}
