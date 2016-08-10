using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging.Messages
{
    [Serializable]
    class ConnectionInfoRequest : AMessage
    {
        public override byte Id { get { return (byte)MessageId.ConnectionInfoRequest; } }

        [MessageField(1)]
        public string ClientName { get; set; }
    }
}
