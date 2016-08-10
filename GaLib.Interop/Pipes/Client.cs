using GaLib.Interop.IPC;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Pipes
{
    class Client
    {
        private readonly PipeStream stream;
        private readonly string pipeName;
        private readonly string serverName;

        private readonly string tag;
        private string serverTag;

        

        public Client(string serverName, string pipeName, string tag)
        {
            this.serverName = serverName;
            this.pipeName = pipeName;
            this.tag = tag;

            IpcClientPipe clientPipe = new IpcClientPipe(serverName, pipeName);

            stream = clientPipe.Connect(1000);
            //stream.BeginRead()
        }
    }
}
