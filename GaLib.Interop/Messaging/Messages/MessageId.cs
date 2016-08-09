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
    enum MessageId
    {
        None = 0,
        MethodInfoRequest = 1,
        MethodInfoAnswer = 2,
        CallRequest = 3,
        CallAnswer = 4
    }
}
