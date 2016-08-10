using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging.Messages
{
    /// <summary>
    /// An enumeration of the valid message ids
    /// </summary>
    class MessageId
    {
        public const byte None = 0;
        public const byte ConnectionInfoRequest = 1;
        public const byte ConnectionInfoAnswer = 2;
        public const byte MethodInfoRequest = 3;
        public const byte MethodInfoAnswer = 4;
        public const byte CallRequest = 5;
        public const byte CallAnswer = 6;
    }
}
