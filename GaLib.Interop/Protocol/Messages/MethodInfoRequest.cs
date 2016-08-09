using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Protocol.Messages
{
    [Serializable]
    class MethodInfoRequest : AMessage
    {
        public Guid DefinitionId { get; private set; }

        public MethodInfoRequest(Guid definitionId)
            : base()
        {
            this.DefinitionId = definitionId;
        }
        
        public override MessageId Id
        {
            get { return MessageId.MethodInfoRequest; }
        }
    }
}
