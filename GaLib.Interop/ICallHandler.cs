using GaLib.Interop.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop
{
    interface ICallHandler
    {
        MethodInfoAnswer HandleMethodInfoRequest(MethodInfoRequest methodInfoRequest);
        void HandleMethodInfoAnswer(MethodInfoAnswer methodInfoAnswer);

        CallAnswer HandleCallRequest(CallRequest callRequest);
        void HandleCallAnswer(CallAnswer callAnswer);
    }
}
