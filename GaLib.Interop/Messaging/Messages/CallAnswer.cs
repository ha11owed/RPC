using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging.Messages
{
    /// <summary>
    /// The response to a method call message
    /// </summary>
    class CallAnswer : AMessage
    {
        public override byte Id { get { return (byte)MessageId.CallAnswer; } }

        [MessageField(0)]
        public Guid TransactionId { get; set; }

        [MessageField(1)]
        public Guid MethodInfoId { get; set; }

        [MessageField(2)]
        public object[] ParameterOutValues { get; set; }

        [MessageField(3)]
        public object ReturnValue { get; set; }

        [MessageField(4)]
        public string ExceptionMessage { get; set; }
    }
}
