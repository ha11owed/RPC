using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GaLib.Interop
{
    public interface IReflectionCaller
    {
        object Call(object target, MethodInfo method, params object[] parameters);

        void RaiseEvent(object target, string name, params object[] parameters);
    }
}
