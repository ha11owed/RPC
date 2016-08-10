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
    class ConnectionInfoAnswer : AMessage
    {
        public override byte Id { get { return (byte)MessageId.ConnectionInfoAnswer; } }

        [MessageField(1)]
        public string ServerName { get; set; }
    }
}
