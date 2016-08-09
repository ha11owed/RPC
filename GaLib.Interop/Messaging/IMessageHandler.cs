using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    interface IMessageHandler
    {
        void Receive(AMessage message);
        void Send(AMessage message);
    }
}
