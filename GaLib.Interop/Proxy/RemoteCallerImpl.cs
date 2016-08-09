using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GaLib.Interop.Proxy
{
    public class RemoteCallerImpl : IReflectionCaller
    {
        public object Call(object target, MethodInfo method, params object[] parameters)
        {
            return method.Invoke(target, parameters);
        }
        
        public void RaiseEvent(object target, string name, params object[] parameters)
        {
        }
    }
}
