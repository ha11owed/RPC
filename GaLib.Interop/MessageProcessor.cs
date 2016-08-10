using GaLib.Interop.Messaging;
using GaLib.Interop.Messaging.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop
{
    class MessageProcessor
    {
        private readonly object sync = new object();

        private readonly string tag;
        private readonly MessageReader reader = new MessageReader();

        private readonly ICallHandler callHandler;
        private readonly List<CallRequest> pendingRequests = new List<CallRequest>(128);

        public string receivedTag;

        public MessageProcessor(ICallHandler callHandler, string tag)
        {
            if (callHandler == null) { throw new ArgumentNullException("callHandler"); }
            this.callHandler = callHandler;
            this.tag = tag;
        }

        public BytesBuffer ProcessResponse(byte[] data, int offset, int count)
        {
            AMessage message = reader.Process(data, offset, count);
            if (message == null)
                return null;

            AMessage result = GetResponse(message);
            if (result == null)
                return null;

            BytesBuffer bb = new BytesBuffer(128);
            result.Serialize(bb);
            return bb;
        }

        public AMessage GetResponse(AMessage message)
        {
            if (message == null)
                return null;

            AMessage result = null;

            switch (message.Id)
            {
                // ---- Call ----
                case MessageId.CallRequest:
                    CallRequest callRequest = (CallRequest)message;
                    result = callHandler.HandleCallRequest(callRequest);
                    if (result == null)
                    {
                        lock (sync)
                        {
                            // Add the call request to the queue
                            pendingRequests.Add(callRequest);
                        }
                        // Send a method info request
                        MethodInfoRequest methodInfoRequest = new MethodInfoRequest();
                        methodInfoRequest.MethodInfoId = callRequest.MethodInfoId;
                        result = methodInfoRequest;
                    }
                    break;
                case MessageId.CallAnswer:
                    callHandler.HandleCallAnswer((CallAnswer)message);
                    break;
                // ---- MethodInfo ----
                case MessageId.MethodInfoRequest:
                    result = callHandler.HandleMethodInfoRequest((MethodInfoRequest)message);
                    break;
                case MessageId.MethodInfoAnswer:
                    MethodInfoAnswer methodInfoAnswer = (MethodInfoAnswer)message;
                    callHandler.HandleMethodInfoAnswer(methodInfoAnswer);

                    CallRequest pendingCallRequest = null;
                    lock (sync)
                    {
                        for (int i = 0; i < pendingRequests.Count; i++)
                        {
                            if (pendingRequests[i].MethodInfoId == methodInfoAnswer.MethodInfoId)
                            {
                                // We got the answer for a method info
                                pendingCallRequest = pendingRequests[i];
                                pendingRequests.RemoveAt(i);
                            }
                        }
                    }
                    if (pendingCallRequest != null)
                    {
                        result = callHandler.HandleCallRequest(pendingCallRequest);
                    }
                    break;
                // ---- ConnectionInfo ----
                case MessageId.ConnectionInfoRequest:
                    result = HandleConnectionInfoRequest((ConnectionInfoRequest)message);
                    break;
                case MessageId.ConnectionInfoAnswer:
                    HandleConnectionInfoAnswer((ConnectionInfoAnswer)message);
                    break;
                default:
                    string errorMsg = string.Format("Messages of Id={0} and type={1} are not supported", message.Id, message.GetType());
                    throw new NotSupportedException(errorMsg);
            }
            return result;
        }

        private ConnectionInfoAnswer HandleConnectionInfoRequest(ConnectionInfoRequest message)
        {
            receivedTag = message.ClientName;

            ConnectionInfoAnswer result = new ConnectionInfoAnswer();
            result.ServerName = tag;
            return result;
        }

        private void HandleConnectionInfoAnswer(ConnectionInfoAnswer message)
        {
            receivedTag = message.ServerName;
        }
    }
}
