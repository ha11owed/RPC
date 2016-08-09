using GaLib.Interop.IPC;
using GaLib.Interop.Protocol.Messages;
using GaLib.Interop.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GaLib.Interop.Protocol
{
    class CallServer
    {
        private Int32 m_count;

        private readonly IObjectMapper mapper;
        private readonly ConcurrentDictionary<Guid, Definition> callDefinitions = new ConcurrentDictionary<Guid, Definition>();

        public CallServer(IObjectMapper mapper)
        {
            this.mapper = mapper;
        }

        

        public void OnAsyncMessage(PipeStream pipe, Byte[] data, Int32 count, Object state)
        {
            Debug.WriteLine("Message received");

            //ConnectionState connState = (ConnectionState)state;
            //BytesBuffer serializer = connState.Serialization;

            //MethodCall methodCall;
            //if (!serializer.TryDeserialize(data, 0, count, out methodCall))
            //{
            //    // Unexpected request, do some error logging, return
            //    return;
            //}

            //Definition callDefinition;
            //if (!callDefinitions.TryGetValue(methodCall.DefinitionId, out callDefinition))
            //{
            //    connState.Queue.Enqueue(methodCall);
            //    // We don't have the definition,
            //    // we first have to fetch it from the client
            //    //data = serializer.SerializeDefinitionRequest(methodCall.DefinitionId);
            //    count = data.Length;
            //}
            //else
            //{
            //    // We have the definition, answer
            //    data = callDefinition.Call(methodCall, mapper, serializer);
            //    count = data.Length;
            //}
            //// Write results
            //try
            //{
            //    pipe.BeginWrite(data, 0, count, OnAsyncWriteComplete, pipe);
            //}
            //catch (Exception)
            //{
            //    pipe.Close();
            //}
        }

        private void OnAsyncWriteComplete(IAsyncResult result)
        {
            PipeStream pipe = (PipeStream)result.AsyncState;
            pipe.EndWrite(result);
        }
    }
}
