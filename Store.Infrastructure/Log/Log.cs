using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Infrastructure.Log
{
    public class Log
    {
        private static log4net.ILog loginfo = log4net.LogManager.GetLogger("loginfo");
        private static log4net.ILog logerr = log4net.LogManager.GetLogger("logerror");


        public static void Info(string msg)
        {
            loginfo.Info(msg);
        }

        public static void Error(string msg,Exception e=null)
        {
            logerr.Error(msg, e);
        }
    }
}
