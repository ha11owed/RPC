using GaLib.Interop.IPC;
using GaLib.Interop.Messaging;
using GaLib.Interop.Messaging.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop
{
    class Dispatcher : IMessageHandler, IpcCallback
    {
        private readonly ConcurrentDictionary<Guid, MethodInfo> methodInfos = new ConcurrentDictionary<Guid, MethodInfo>();

        private readonly IObjectMapper mapper;
        private readonly IpcClientPipe client;

        public Dispatcher(IObjectMapper mapper, string serverName, string serverToken)
        {
            this.mapper = mapper;
            client = new IpcClientPipe(serverName, serverToken);
        }

        public CallAnswer Process(CallRequest request)
        {
            if (request == null) { throw new ArgumentNullException("request"); }

            CallAnswer result = new CallAnswer();

            //methodInfos.GetOrAdd(request.MethodInfoId, )

            return result;
        }

        public void Receive(AMessage message)
        {
        }

        public void Send(AMessage message)
        {

        }

        void IpcCallback.OnAsyncConnect(PipeStream pipe, out object state)
        {
            state = null;
        }

        void IpcCallback.OnAsyncDisconnect(PipeStream pipe, object state)
        {
        }

        void IpcCallback.OnAsyncMessage(PipeStream pipe, byte[] data, int bytes, object state)
        {
            
        }
    }
}
