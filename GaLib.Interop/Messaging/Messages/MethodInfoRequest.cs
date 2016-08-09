using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging.Messages
{
    /// <summary>
    /// A message requesting a method info declaration
    /// </summary>
    class MethodInfoRequest : AMessage
    {
        public override byte Id { get { return (byte)MessageId.MethodInfoRequest; } }

        [MessageField(1)]
        public Guid MethodInfoId { get; set; }
    }
}
