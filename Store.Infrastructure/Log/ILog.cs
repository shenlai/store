using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure.Log
{
    public interface ILog
    {
        void Info(string msg);
        //void Waring(string msg);
        //void Debug(string msg);
        void Error(string msg,Exception e);
        //void Exception(Exception e);

    }
}
