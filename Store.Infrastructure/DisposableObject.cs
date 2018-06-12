using System;

namespace Store.Infrastructure
{
    public abstract class DisposableObject:IDisposable
    {
        ~DisposableObject()
        {
            this.Dispose(false);
        }

        protected abstract void Dispose(bool disposing);

        protected void ExplicitDispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);//请求系统不要调用指定对象的终结器
            //就是告诉垃圾回收器不要调用指定对象的Dispose方法，因为之前Dispose(true);已经做过了。
            //防止两次执行。
        }

        public void Dispose()
        {
            this.ExplicitDispose();
        }
    }
}
