using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Serialization
{
    [Serializable]
    struct DefinitionAnswer
    {
        public Guid DefinitionId { get; set; }

        public Identifier Target { get; set; }
        public string MethodName { get; set; }

        public Identifier[] Parameters { get; set; }
        public Identifier ReturnType { get; set; }
    }
}
