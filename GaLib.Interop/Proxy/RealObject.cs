using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GaLib.Interop.Proxy
{
    /// <summary>
    /// Container for the objects that are being used as proxy somewhere.
    /// 
    /// The class holds the real object instance and:
    ///     1. watches for triggered events.
    ///     2. dispatches the events to the proxies.
    ///     3. executes the proxy operations on the real instance.
    /// </summary>
    public sealed class RealObject : IDisposable
    {
        private readonly Guid identity;
        private readonly object target;
        private readonly Dictionary<EventInfo, Delegate> eventToHandlers = new Dictionary<EventInfo, Delegate>();

        private readonly IReflectionCaller remoteCaller;

        public RealObject(IReflectionCaller remoteCaller, object target)
        {
            this.identity = Guid.NewGuid();

            this.remoteCaller = remoteCaller;
            this.target = target;

            RegisterHandlers();
        }

        /// <summary>
        /// The object was destroyed. No other call should be allowed
        /// </summary>
        public void Dispose()
        {
            UnregisterHandlers();
        }

        /// <summary>
        /// Get the identity of the real object
        /// </summary>
        public Guid Identity { get { return identity; } }

        /// <summary>
        /// Invoke a method call on the real object
        /// </summary>
        public object Invoke(MethodInfo method, params object[] parameters)
        {
            return method.Invoke(target, parameters);
        }

        private void RegisterHandlers()
        {
            Type type = target.GetType();
            foreach (EventInfo eventInfo in type.GetEvents())
            {
                Delegate handler = CreateEventHandler(eventInfo);
                eventToHandlers[eventInfo] = handler;
                eventInfo.AddEventHandler(target, handler);
            }
        }

        private void UnregisterHandlers()
        {
            foreach (var kv in eventToHandlers)
            {
                EventInfo eventInfo = kv.Key;
                Delegate handler = kv.Value;
                eventInfo.RemoveEventHandler(target, handler);
            }
            eventToHandlers.Clear();
        }

        private static void HandleStandardEvent(RealObject proxiedObject, EventInfo eventInfo, object source, EventArgs e)
        {
            object target = proxiedObject.target;
            var remote = proxiedObject.remoteCaller;

            //FieldInfo eventField = target.GetType().GetField(eventInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            //MulticastDelegate handler = eventField.GetValue(target);
            //remote.Call(target, eventInfo.GetRaiseMethod(), source, e);
            remote.RaiseEvent(target, eventInfo.Name, source, e);
        }

        /// <summary>
        /// Create an event handler that will call one of the handle event methods
        /// </summary>
        private Delegate CreateEventHandler(EventInfo eventInfo)
        {
            LambdaExpression methodExpression = null;

            Type targetType = GetType();
            Type delegateType = eventInfo.EventHandlerType;

            MethodInfo mi = delegateType.GetMethod("Invoke");
            ParameterInfo[] parameterInfos = mi.GetParameters();

            if (parameterInfos.Length == 2
                && parameterInfos[0].ParameterType == typeof(object)
                && typeof(EventArgs).IsAssignableFrom(parameterInfos[1].ParameterType))
            {
                // Standard event handler
                ParameterInfo p0 = parameterInfos[0];
                ParameterInfo p1 = parameterInfos[1];

                ParameterExpression source = Expression.Parameter(p0.ParameterType, p0.Name);
                ParameterExpression args = Expression.Parameter(p1.ParameterType, p1.Name);

                MethodInfo handleEvent = targetType.GetMethod("HandleStandardEvent", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                MethodCallExpression callExpression = Expression.Call(
                    handleEvent,
                    Expression.Constant(this, targetType),
                    Expression.Constant(eventInfo, typeof(EventInfo)),
                    Expression.Convert(source, typeof(object)),
                    Expression.Convert(args, typeof(EventArgs)));

                methodExpression = Expression.Lambda(delegateType, callExpression, source, args);
            }
            else
            {
                throw new NotImplementedException();
                //for (int i = 0; i < parameterInfos.Length; i++)
                //{
                //    ParameterInfo pi = parameterInfos[i];
                //    ParameterExpression pe = Expression.Parameter(pi.ParameterType, pi.Name);
                //    parameters[i] = pe;
                //}
                //Expression callExpression = Expression.Call(
                //    targetType,
                //    methodName,
                //    null,
                //    parameters);
                //Expression<Delegate> methodExpression = Expression.Lambda<Delegate>(callExpression, parameters);
            }
            Delegate method = methodExpression.Compile();
            return method;
        }
    }
}
