using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Protocol
{
    interface IMessage
    {
        MessageId Id { get; }
    }
}
