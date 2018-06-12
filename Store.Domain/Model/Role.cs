using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Domain.Model
{
    public class Role : AggregateRoot
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
