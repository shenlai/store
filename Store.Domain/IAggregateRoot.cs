using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain
{
    //聚合根接口,继承与该接口的对象是外部唯一操作的对象
    public interface IAggregateRoot:IEntity
    {
    }
}
