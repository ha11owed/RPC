using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.Serialization
{
    [Serializable]
    struct MethodCall
    {
        public Guid Id { get; set; }
        public Guid DefinitionId { get; set; }

        public byte[] Target { get; set; }
        public byte[][] ParameterValues { get; set; }
    }
}
