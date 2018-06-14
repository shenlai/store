using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

//参考来源：https://github.com/KidFashion/redlock-cs
namespace Store.Redlock
{
    public class RedLock
    {

        public RedLock(params ConnectionMultiplexer[] list)
        {
            foreach (var item in list)
                this.redisMasterDictionary.Add(item.GetEndPoints().First().ToString(), item);
        }

        const int DefaultRetryCount = 3;
        readonly TimeSpan DefaultRetryDelay = new TimeSpan(0, 0, 0, 0, 200);
        //计算偏移时间
        const double ClockDriveFactor = 0.01;

        //必须成功获取的锁数量：用N/2+1当标准(N为实例数)
        protected int Quorum { get { return (redisMasterDictionary.Count / 2) + 1; } }

        /// <summary>
        /// String containing the Lua unlock script.
        /// 判断客户端传过来的value和锁对象的value是否一样,只有拿到锁的客户端才能进行删除
        /// </summary>
        const String UnlockScript = @"
            if redis.call(""get"",KEYS[1]) == ARGV[1] then
                return redis.call(""del"",KEYS[1])
            else
                return 0
            end";


        protected static byte[] CreateUniqueLockId()
        {
            return Guid.NewGuid().ToByteArray();
        }

        //redis集群
        //protected Dictionary<String,ConnectionMultiplexer> redisMasterDictionary = new Dictionary<string,ConnectionMultiplexer>();
        public Dictionary<String, ConnectionMultiplexer> redisMasterDictionary = new Dictionary<string, ConnectionMultiplexer>();

        //TODO: Refactor passing a ConnectionMultiplexer
        protected bool LockInstance(string redisServer, string resource, byte[] val, TimeSpan ttl)
        {

            bool succeeded;
            try
            {
                var redis = this.redisMasterDictionary[redisServer];
                //每个实例上获取锁,已存(该锁已被其他资源占用)在则返回失败
                succeeded = redis.GetDatabase().StringSet(resource, val, ttl, When.NotExists);
            }
            catch (Exception)
            {
                succeeded = false;
            }
            return succeeded;
        }

        //TODO: Refactor passing a ConnectionMultiplexer
        protected void UnlockInstance(string redisServer, string resource, byte[] val)
        {
            RedisKey[] key = { resource };
            RedisValue[] values = { val };
            var redis = redisMasterDictionary[redisServer];
            //判断客户端传过来的value和锁对象的value是否一样,只有拿到锁的客户端才能进行删除
            redis.GetDatabase().ScriptEvaluate(
                UnlockScript,
                key,
                values
                );
        }

        /// <summary>
        /// Redlock算法
        /// </summary>
        /// <param name="resource">key</param>
        /// <param name="ttl">锁自动释放时间</param>
        /// <param name="lockObject">返回一个锁</param>
        /// <returns></returns>
        public bool Lock(RedisKey resource, TimeSpan ttl, out Lock lockObject)
        {
            var val = CreateUniqueLockId();
            Lock innerLock = null;
            bool successfull = retry(DefaultRetryCount, DefaultRetryDelay, () =>
            {
                try
                {
                        //成功获取到的锁数量
                        int n = 0;
                        //记录开始时间
                        var startTime = DateTime.Now;

                        //使用同样key和值，循环在多个实例中获得锁
                        for_each_redis_registered(
                        redis =>
                        {
                            if (LockInstance(redis, resource, val, ttl))
                            {
                                n += 1;
                            }
                        }
                    );

                        //偏移时间：锁自动释放时间的1%+2
                        var drift = Convert.ToInt32((ttl.TotalMilliseconds * ClockDriveFactor) + 2);
                        //锁对象的有效时间 = 锁自动释放时间-(当前时间-开始时间=程序已执行时间)-偏移时间
                        var validity_time = ttl - (DateTime.Now - startTime) - new TimeSpan(0, 0, 0, 0, drift);

                        //判断成功的数量和有效时间c值是否大于0,则获取锁成功，否则失败
                        if (n >= Quorum && validity_time.TotalMilliseconds > 0)
                    {
                        innerLock = new Lock(resource, val, validity_time);
                        return true;
                    }
                    else
                    {
                            //获取锁失败，释放已获取的锁
                            for_each_redis_registered(
                            redis =>
                            {
                                UnlockInstance(redis, resource, val);
                            }
                        );
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            });

            lockObject = innerLock;
            return successfull;
        }

        protected void for_each_redis_registered(Action<ConnectionMultiplexer> action)
        {
            foreach (var item in redisMasterDictionary)
            {
                action(item.Value);
            }
        }

        //使用同样key和值，循环在多个实例中获得锁
        protected void for_each_redis_registered(Action<String> action)
        {
            foreach (var item in redisMasterDictionary)
            {
                action(item.Key);
            }
        }

        //获取锁，失败尝试重新获取retryCount次
        protected bool retry(int retryCount, TimeSpan retryDelay, Func<bool> action)
        {
            int maxRetryDelay = (int)retryDelay.TotalMilliseconds; //200ms
            Random rnd = new Random();
            int currentRetry = 0;

            while (currentRetry++ < retryCount)  //3次
            {
                if (action()) return true;
                //获取锁失败，间隔rnd.Next(maxRetryDelay)毫秒后，重新获取锁
                //Thread.Sleep(rnd.Next(maxRetryDelay));
                Thread.Sleep(5000);
            }
            return false;
        }

        public void Unlock(Lock lockObject)
        {
            for_each_redis_registered(redis =>
            {
                UnlockInstance(redis, lockObject.Resource, lockObject.Value);
            });
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(this.GetType().FullName);

            sb.AppendLine("Registered Connections:");
            foreach (var item in redisMasterDictionary)
            {
                sb.AppendLine(item.Value.GetEndPoints().First().ToString());
            }

            return sb.ToString();
        }
    }
}
