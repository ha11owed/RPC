using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.Serialization
{
    [Serializable]
    struct DefinitionRequest
    {
        public Guid DefinitionId { get; set; }
    }
}
