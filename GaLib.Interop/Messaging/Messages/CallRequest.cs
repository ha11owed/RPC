using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging.Messages
{
    /// <summary>
    /// A request for a method call
    /// </summary>
    class CallRequest : AMessage
    {
        public override byte Id { get { return (byte)MessageId.CallRequest; } }

        [MessageField(0)]
        public Guid TransactionId { get; set; }

        [MessageField(1)]
        public Guid MethodInfoId { get; set; }

        [MessageField(2)]
        public object Target { get; set; }
        [MessageField(3)]
        public object[] ParameterValues { get; set; }
    }
}
