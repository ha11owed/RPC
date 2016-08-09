using GaLib.Interop.Messaging;
using GaLib.Interop.Messaging.Messages;
using GaLib.Interop.Proxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GaLib.Interop.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestMessageSerialization();
            //TestProxy();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey(true);
        }

        private static void TestMessageSerialization()
        {
            CallRequest msg1 = new CallRequest();
            msg1.MethodInfoId = Guid.NewGuid();
            msg1.Target = Guid.NewGuid();
            msg1.ParameterValues = new object[] { 1, 2, 3 };
            byte[] data = msg1.Compile();

            CallRequest msg2 = (CallRequest)MessageManager.Create(msg1.Id, data);

            Debug.Assert(msg2.Id == msg1.Id);
            Debug.Assert(object.Equals(msg1.Target, msg2.Target));
            Debug.Assert(msg1.ParameterValues.Length == msg2.ParameterValues.Length);

            CallRequest msg3;
            using (MemoryStream ms = new MemoryStream())
            {
                msg1.BeginWrite(ms);

                Thread.Sleep(100);
                ms.Position = 0;

                var ar = AMessage.BeginRead(ms);
                Thread.Sleep(100);

                msg3 = ar.GetMessage() as CallRequest;
            }

            Debug.Assert(msg1.Id == msg3.Id);
            Debug.Assert(object.Equals(msg1.Target, msg3.Target));
            Debug.Assert(msg1.ParameterValues.Length == msg3.ParameterValues.Length);

            var mainMethodInfo = typeof(Program).GetMethod("Main", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            MethodInfoAnswer miAnswer1 = new MethodInfoAnswer();
            miAnswer1.MethodInfoId = Guid.NewGuid();
            miAnswer1.MethodInfo = mainMethodInfo;
            byte[] miAnswerBytes = miAnswer1.Compile();
            MethodInfoAnswer miAnswer2 = (MethodInfoAnswer)AMessage.Create(miAnswer1.Id, miAnswerBytes);

            Debug.Assert(miAnswer1.MethodInfo == miAnswer2.MethodInfo);
        }

        private static void TestProxy()
        {
            RemoteCallerImpl remoteCaller = new RemoteCallerImpl();

            var testImpl = new TestImpl();
            RealObject po = new RealObject(remoteCaller, testImpl);
            testImpl.PropertyChanged += (s, e) =>
            {
                Console.Write("Test");
            };
            testImpl.OnPropertyChanged();

            Proxify proxify = new Proxify();
            ITest test = proxify.CreateProxy(typeof(ITest), Guid.NewGuid(), remoteCaller) as ITest;
            test.PropertyChanged += (s, e) =>
            {
                Console.Write("Test");
            };
            ITest test2 = proxify.CreateProxy(typeof(ITest), Guid.NewGuid(), remoteCaller) as ITest;
            string bp = test.BaseProp;

            test.PropertyChanged += test_PropertyChanged;
            int v = test.Test();

            string bp2 = test.BaseProp;

            bool eq = test.Equals(test2);
        }

        static void test_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }
    }
}
