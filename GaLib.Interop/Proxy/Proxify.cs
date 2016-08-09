using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace GaLib.Interop.Proxy
{
    public class Proxify
    {
        class ProxifyInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method == null)
                    return;

                ProxyObject target = invocation.Proxy as ProxyObject;
                if (target == null)
                    return;

                MethodInfo method = invocation.Method;

                invocation.ReturnValue = target.DoMethodCall(method, invocation.Arguments);
            }
        }

        private static readonly ProxifyInterceptor Interceptor = new ProxifyInterceptor();

        public T CreateProxy<T>(Guid identity, IReflectionCaller remoteCaller)
        {
            return (T)CreateProxy(typeof(T), identity, remoteCaller);
        }

        public object CreateProxy(Type targetType, Guid identity, IReflectionCaller remoteCaller)
        {
            Type[] interfaces = GetTopInterfaces(targetType);

            ProxyGenerationOptions options = new ProxyGenerationOptions();
            options.BaseTypeForInterfaceProxy = typeof(ProxyObject);
            ProxyGenerator generator = new ProxyGenerator();
            ProxyObject proxy = (ProxyObject)generator.CreateInterfaceProxyWithoutTarget(interfaces[0], interfaces.Skip(1).ToArray(), options, Interceptor);
            proxy.Init(identity, remoteCaller);
            return proxy;
        }

        /// <summary>
        /// Get the top level interfaces of a type.
        /// This are interfaces that are implemented by the type but not by another interface of the type
        /// </summary>
        private static Type[] GetTopInterfaces(Type type)
        {
            List<Type> interfaces = new List<Type>();
            List<Type> subInterfaces = new List<Type>();

            if (type.IsInterface)
                interfaces.Add(type);
            interfaces.AddRange(type.GetInterfaces());

            List<Type> temp = interfaces;
            while (temp.Count > 0)
            {
                temp = temp.SelectMany(i => i.GetInterfaces()).ToList();
                subInterfaces.AddRange(temp);
            }

            subInterfaces = subInterfaces.Distinct().ToList();
            Type[] result = interfaces.Except(subInterfaces).ToArray();
            return result;
        }
    }
}
