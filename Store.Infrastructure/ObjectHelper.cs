using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure
{
    public class ObjectHelper
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore, ContractResolver = new WritablePropertiesOnlyResolver() };

        /// <summary>
        /// 不同对象之间的深拷贝，最好属性名一样
        /// </summary>
        /// <typeparam name="T">源对象类型</typeparam>
        /// <typeparam name="F">目的对象类型</typeparam>
        /// <param name="original">源对象</param>
        /// <returns>目的对象</returns>
        public static F DeepCopy<T, F>(T original) where F : class where T:class
        {
            try
            {
                var jsonStr = JsonConvert.SerializeObject(original, JsonSerializerSettings);
                var result = JsonConvert.DeserializeObject<F>(jsonStr, JsonSerializerSettings);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static void DeepCopy<T, F>(T original, ref F destination)
            where T:class
            where F:class
        {
            destination = DeepCopy<T, F>(original);
        }





    }

    /// <summary>
    /// http://www.cnblogs.com/hao-dotnet/p/4229825.html
    /// 可扩展
    /// </summary>
    public class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.Writable).ToList();
        }
    }
}
