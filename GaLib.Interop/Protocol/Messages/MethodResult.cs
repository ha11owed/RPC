using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Serialization
{
    [Serializable]
    struct MethodResult
    {
        public Guid Id { get; set; }
        public byte[] Value { get; set; }
    }
}
