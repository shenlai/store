using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain
{
    //领域实体接口
    public interface IEntity
    {
        //当前领域实体的唯一标示
        Guid Id { get; set; }
    }
}
