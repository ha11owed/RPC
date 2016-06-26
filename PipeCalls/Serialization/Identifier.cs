using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.Serialization
{
    [Serializable]
    struct Identifier
    {
        public string ClassName { get; set; }
        public IdentifierType Type { get; set; }

        public Identifier()
        {
            this.ClassName = null;
            this.Type = IdentifierType.None;
        }

        public Identifier(string className, IdentifierType type)
        {
            this.ClassName = className;
            this.Type = type;
        }
    }
}
