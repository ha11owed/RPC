using GaLib.Interop.IPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop
{
    class RpcClient
    {
        private readonly IObjectMapper mapper;
        private readonly IpcClientPipe client;
        private readonly IpcServer server;

        public RpcClient(IObjectMapper mapper, string serverName, string pipe)
        {
            client = new IpcClientPipe(serverName, "c" + pipe);
            //server = new IpcServer("s" + pipe, new RpcServerCallback(mapper), 1);

            var stream = client.Connect(1000);
            
            //stream.BeginRead(null, 0, 1, )
        }
    }
}
