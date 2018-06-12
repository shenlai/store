using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure.InterceptionBehaviors
{
    public class ExceptionLoggingBehavior : IInterceptionBehavior
    {
        /// <summary>
        /// 表示当拦截行为被调用时，是否需要执行某些操作
        /// </summary>
        public bool WillExecute { get { return true; } }

        /// <summary>
        /// 需要拦截的对象类型接口
        /// </summary>
        /// <returns></returns>      
        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        /// <summary>
        /// 拦截调用并执行所需的拦截行为
        /// </summary>
        /// <param name="input">调用拦截目标时输入信息</param>
        /// <param name="getNext">通过行为链获取下一个拦截行为的委托</param>
        /// <returns>从拦截目标获得的返回信息</returns>
        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            //执行目标方法
            var methodReturn = getNext().Invoke(input, getNext);
            //方法执行后处理
            if (methodReturn.Exception != null)
            {
                Log.Log.Error(null, methodReturn.Exception);
            }
            return methodReturn;
        }
    }
}
