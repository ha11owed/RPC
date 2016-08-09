using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Serialization
{
    public class SerializationCaller : IReflectionCaller
    {
        private readonly object sync = new object();
        private readonly Dictionary<Guid, DefinitionAnswer> callDefinitions = new Dictionary<Guid, DefinitionAnswer>();

        private readonly Stream stream;
        private readonly IObjectMapper mapper;

        public SerializationCaller(Stream stream, IObjectMapper mapper)
        {
            this.stream = stream;
            this.mapper = mapper;
        }


        public object Call(object target, MethodInfo method, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public void RaiseEvent(object target, string name, params object[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
