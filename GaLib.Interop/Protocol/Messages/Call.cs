using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Protocol.Messages
{
    [Serializable]
    class Call : AMessage
    {
        public override MessageId Id { get { return MessageId.CallRequest; } }

        [Field(1)]
        public Guid DefinitionId { get; set; }

        [Field(2)]
        public object Target { get; set; }
        [Field(3)]
        public object[] ParameterValues { get; set; }
    }
}
