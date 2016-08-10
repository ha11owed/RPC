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
    class CallHandler : ICallHandler
    {
        private readonly ConcurrentDictionary<Guid, MethodInfo> methodInfos = new ConcurrentDictionary<Guid, MethodInfo>();

        private readonly IObjectMapper mapper;
        private readonly IpcClientPipe client;

        public CallHandler(IObjectMapper mapper, string serverName, string serverToken)
        {
            this.mapper = mapper;
            client = new IpcClientPipe(serverName, serverToken);
        }

        public MethodInfoAnswer HandleMethodInfoRequest(MethodInfoRequest methodInfoRequest)
        {
            
            throw new NotImplementedException();
        }

        public void HandleMethodInfoAnswer(MethodInfoAnswer methodInfoAnswer)
        {
            throw new NotImplementedException();
        }

        public CallAnswer HandleCallRequest(CallRequest callRequest)
        {
            CallAnswer result = null;
            MethodInfo methodInfo;
            methodInfos.TryGetValue(callRequest.MethodInfoId, out methodInfo);
            if (methodInfo != null)
            {
                callRequest.Target = GetRealObject(callRequest.Target);
                object[] paramValues = callRequest.ParameterValues;
                for (int i = 0; i < paramValues.Length; i++)
                {
                    paramValues[i] = GetRealObject(paramValues[i]);
                }

                result = new CallAnswer();

                try
                {
                    object returnValue = methodInfo.Invoke(callRequest.Target, paramValues);
                    result.ReturnValue = GetObjectId(returnValue);
                    //foreach()
                }
                catch (Exception ex)
                {
                    result.ExceptionMessage = ex.Message;
                }
            }
            return result;
        }

        public void HandleCallAnswer(CallAnswer callAnswer)
        {
            throw new NotImplementedException();
        }

        private object GetRealObject(object referenceId)
        {
            return null;
        }

        private object GetObjectId(object referenceId)
        {
            return null;
        }
    }
}
