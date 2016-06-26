using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.Serialization
{
    enum MessageType : byte
    {
        None = 0,
        GetObject = 1,
        MethodDescriptionRequest = 20,
        MethodDescriptionAnswer = 21,
    }
}
