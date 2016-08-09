using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging.Messages
{
    /// <summary>
    /// A message answering with a method info declaration
    /// </summary>
    class MethodInfoAnswer : AMessage
    {
        public override byte Id { get { return (byte)MessageId.MethodInfoAnswer; } }

        [MessageField(1)]
        public Guid MethodInfoId { get; set; }

        [MessageField(2)]
        public MethodInfo MethodInfo { get; set; }
    }
}
