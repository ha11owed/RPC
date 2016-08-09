using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Test.Proxy
{
    public interface ITest1
    {
        string Name { get; set; }
        string DoSomething();

        event EventHandler BasicEvent;

        int this[string index] { get; set; }
    }
}
