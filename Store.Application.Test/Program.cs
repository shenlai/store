using Store.Service.Wcf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Application.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ProductService se = new ProductService();
            Console.Write("调用wcf结果：{0}", se.sum(12));
            Console.Read();
        }
    }
}
