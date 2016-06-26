using PipeCalls.IPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.Proxy
{
    public class ProxyGenerator
    {
        private static readonly Type commType = typeof(IpcClientPipe);

        public static T CreateInstance<T>()
        {
            Type type = typeof(T);

            TypeBuilder builder = new TypeBuilder();
            builder.AddInterfaceImplementation(type);

            FieldBuilder commBuilder = builder.DefineField("communication", commType, FieldAttributes.Private);
            ConstructorBuilder constructor = builder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { commType });

            var generator = constructor.GetILGenerator();
            //generator.Emit()
            //constructor.GetMethodBody().


        }
    }
}
