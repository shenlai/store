using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;

namespace Store.Events.Bus
{
    public class MsmqBusOptions
    {
        public bool SharedModeDenyReceive { get; set; }
        public bool EnableCache { get; set; }

        //枚举
        public QueueAccessMode QueueAccessMode { get; set; }
        public string Path { get; set; }
        public bool UserInternalTransaction { get; set; }

        public MsmqBusOptions(string path, bool sharedModeDenyReceive, bool enableCache, QueueAccessMode queueAccessMode, bool useInternalTransaction)
        {
            this.SharedModeDenyReceive = sharedModeDenyReceive;
            this.EnableCache = enableCache;
            this.QueueAccessMode = queueAccessMode;
            this.Path = path;
            this.UserInternalTransaction = useInternalTransaction;
        }
        public MsmqBusOptions(string path)
            : this(path, false, false, QueueAccessMode.SendAndReceive, false)
        { }
        public MsmqBusOptions(string path, bool useInternalTransaction)
            :this(path,false,false,QueueAccessMode.SendAndReceive,useInternalTransaction)
        { 
        }

        


    }
}
