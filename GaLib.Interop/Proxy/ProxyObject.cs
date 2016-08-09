using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GaLib.Interop.Proxy
{
    /// <summary>
    /// Represents the base implementation for all the proxy classes.
    /// An instance of this type holds the identity of the real object (but not the real obj)
    /// </summary>
    public class ProxyObject
    {
        private Guid indentity;
        private IReflectionCaller remoteCaller;

        private readonly EventHandlerList eventHandlerList = new EventHandlerList();
        // caches for the object properties
        private readonly Dictionary<MethodInfo, object> getterCache = new Dictionary<MethodInfo, object>();
        // caches for standard object 
        private string toString = null;
        private int? cachedHashCode = null;

        private bool isCallInProgress = false;

        /// <summary>
        /// Set the object identiy
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="remoteCaller"></param>
        public void Init(Guid identity, IReflectionCaller remoteCaller)
        {
            this.indentity = identity;
            this.remoteCaller = remoteCaller;
        }

        public override bool Equals(object obj)
        {
            bool eq;
            IProxyTargetAccessor proxy = this as IProxyTargetAccessor;
            if (proxy == null)
            {
                eq = base.Equals(obj);
            }
            else
            {
                var target = proxy.DynProxyGetTarget();
                if (target == null)
                    eq = base.Equals(obj);
                else
                    eq = target.Equals(obj);
            }
            return eq;
        }

        public override int GetHashCode()
        {
            if (cachedHashCode == null)
            {
                IProxyTargetAccessor proxy = this as IProxyTargetAccessor;
                if (proxy == null)
                {
                    cachedHashCode = base.GetHashCode();
                }
                else
                {
                    var target = proxy.DynProxyGetTarget();
                    if (target == null)
                        cachedHashCode = base.GetHashCode();
                    else
                        cachedHashCode = target.GetHashCode();
                }
            }
            // return the cached value
            return cachedHashCode.Value;
        }

        public override string ToString()
        {
            if (toString == null)
            {
                IProxyTargetAccessor proxy = this as IProxyTargetAccessor;
                if (proxy == null)
                {
                    toString = base.ToString();
                }
                else
                {
                    var target = proxy.DynProxyGetTarget();
                    if (target == null)
                        toString = base.ToString();
                    else
                        toString = target.ToString();
                }
            }
            // return the cached value
            return toString;
        }

        public void RaiseEvent(string name, object[] parameters)
        {
            Delegate handler = eventHandlerList[name];
            if (handler != null)
                handler.DynamicInvoke(parameters);
        }

        public object DoMethodCall(MethodInfo method, object[] parameters)
        {
            if (isCallInProgress)
                return null;

            object result = null;
            try
            {
                isCallInProgress = false;
                bool isGetProp = method.IsSpecialName && method.Name.StartsWith("get_") && parameters.Length == 0;
                if (isGetProp)
                {
                    object cached;
                    if (getterCache.TryGetValue(method, out cached))
                    {
                        return cached;
                    }
                }

                // Do a normal call over the current interface
                result = DoMethodCallInternal(method, parameters);
                if (isGetProp)
                {
                    getterCache[method] = result;
                }
                else
                {
                    getterCache.Clear();
                }
            }
            finally
            {
                isCallInProgress = false;
            }
            return result;
        }

        private object DoMethodCallInternal(MethodInfo method, object[] parameters)
        {
            Type retType = method.ReturnType;
            if (retType == typeof(string))
            {
                return "some string";
            }
            else if (retType == typeof(int))
            {
                return 32242;
            }
            if (method.IsSpecialName)
            {
                int opId = -1;
                string name = null;

                string[] specialOperations = new string[] { "add_", "remove_" };
                for (int i = 0; i < specialOperations.Length; i++)
                {
                    string prefix = specialOperations[i];
                    if (method.Name.StartsWith(prefix))
                    {
                        name = method.Name.Substring(prefix.Length);
                        opId = i;
                        break;
                    }
                }

                switch (opId)
                {
                    case 0:
                        // Add Event Handler
                        eventHandlerList.AddHandler(name, parameters[0] as Delegate);
                        return null;
                    case 1:
                        // Remove Event Handler
                        eventHandlerList.RemoveHandler(name, parameters[0] as Delegate);
                        return null;
                }
            }
            // Default operation
            return remoteCaller.Call(this, method, parameters);
        }
    }
}
