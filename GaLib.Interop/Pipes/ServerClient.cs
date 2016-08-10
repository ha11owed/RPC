using GaLib.Interop.IPC;
using GaLib.Interop.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Pipes
{
    class ServerClient
    {
        private string name;

        private readonly MessageReader reader = new MessageReader();

        public MessageReader Reader { get { return reader; } }
    }
}
