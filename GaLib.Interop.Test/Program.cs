using GaLib.Interop.Protocol.Messages;
using GaLib.Interop.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TestMessageSerialization();
            //TestProxy();

            Console.ReadKey(true);
        }

        private static void TestMessageSerialization()
        {
            Call msg = new Call();
            msg.DefinitionId = Guid.NewGuid();
            msg.Target = Guid.NewGuid();
            msg.ParameterValues = new object[] { 1, 2, 3 };
            byte[] data = msg.Compile();
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
