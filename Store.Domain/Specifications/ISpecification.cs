using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Specifications
{
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T candidate);

        //获得规约表达式树
        Expression<Func<T, bool>> Expression { get; }
    }
}
