using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Serialization
{
    [Serializable]
    struct Identifier
    {
        public string ClassName { get; set; }
        public IdentifierType Type { get; set; }

        public Identifier(string className, IdentifierType type) 
            : this()
        {
            this.ClassName = className;
            this.Type = type;
        }
    }
}
