using Microsoft.Practices.EnterpriseLibrary.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure.Caching
{
    #region
    // 表示基于Microsoft Patterns & Practices - Enterprise Library Caching Application Block的缓存机制的实现
    // 该类简单理解为对Enterprise Library Caching中的CacheManager封装
    // 该缓存实现不支持分布式缓存，更多信息参考: 
    // http://stackoverflow.com/questions/7799664/enterpriselibrary-caching-in-load-balance 
    #endregion
    public class EntLibCacheProvider:ICacheProvider
    {
        //// 获得CacheManager实例，该实例的注册通过cachingConfiguration进行注册进去的，具体看配置文件
        private readonly ICacheManager _cacheManager = CacheFactory.GetCacheManager();

        public void Add(string key, string valueKey, object value)
        {
            Dictionary<string, object> dict = null;
            if(_cacheManager.Contains(key))
            {
                dict = _cacheManager[key] as Dictionary<string, object>;
                dict[valueKey] = value;
            }else
            {
                dict = new Dictionary<string, object>() { { valueKey, value } };
            }

            _cacheManager.Add(key, dict);
        }

        public void Update(string key, string valueKey, object value)
        {
            Add(key, valueKey, value);
        }

        public object Get(string key, string valueKey)
        {
            if (!_cacheManager.Contains(key))
                return null;
            Dictionary<string, object> dict = _cacheManager[key] as Dictionary<string, object>;
            if (dict != null && dict.ContainsKey(valueKey))
                return dict[valueKey];
            return null;
            //return dict == null ? null : dict[valueKey];
        }
        public void Remove(string key)
        {
            _cacheManager.Remove(key);
        }
        public bool Exists(string key)
        {
            return _cacheManager.Contains(key);
        }

        // 判断指定的键值和缓存键值的缓存是否存在
        public bool Exists(string key, string valueKey)
        {
            return Exists(key) && (_cacheManager[key] as Dictionary<string, object>).ContainsKey(valueKey);
        }

          
    }
}
