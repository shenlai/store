using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Store.ServiceContracts.ModelDTOs
{
    /*关于wcf 异常信息抛出：http://www.cnblogs.com/jfzhu/p/4060666.html */
    //创建一个自定义SOAP Fault类
    [DataContract]
    public class FaultData
    {
        #region Public Properties

        [DataMember(Order = 0)]
        public string Message { get; set; }

        [DataMember(Order = 1)]
        public string FullMessage { get; set; }

        [DataMember(Order = 2)]
        public string StackTrace { get; set; }
        #endregion 

        #region Public Static Methods

        public static FaultData CreateFromException(Exception ex)
        {
            return new FaultData
            {
                Message = ex.Message,
                FullMessage = ex.ToString(),
                StackTrace = ex.StackTrace
            };
        }

        public static FaultReason CreateFaultReason(Exception ex)
        {
            return new FaultReason(ex.Message);
        }

        #endregion 

    }
}
