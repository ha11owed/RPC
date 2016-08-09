using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Protocol
{
    public enum MessageId
    {
        MethodInfoRequest = 0,
        MethodInfoAnswer = 1,
        CallRequest = 2,
        CallAnswer = 3
    }
}
