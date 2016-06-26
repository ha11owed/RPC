using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.Serialization
{
    enum CallType
    {
        None = 0,
        MethodCall = 1,
        PropertyGet = 2,
        PropertySet = 3,
        AddEvent = 4,
        RemoveEvent = 5,
    }
}
