using System;
using System.Runtime.Serialization;
namespace Store.Domain
{
    public class DomainException:Exception
    {
        public DomainException()
            : base()
        { }

        public DomainException(string message)
            : base(message)
        { }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /******************params  可变参数个数*********************/
        public DomainException(string format, params object[] args)
            : base(string.Format(format, args))
        { }

        public DomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
