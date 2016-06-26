using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.Serialization
{
    class Serializer
    {
        private readonly IFormatter formatter = new BinaryFormatter();

        public byte[] Serialize(object obj, Identifier identifier, IObjectMapper mapper)
        {
            return null;
        }

        public object Deserialize(byte[] data, Identifier identifier, IObjectMapper mapper)
        {
            object result = null;
            using (MemoryStream stream = new MemoryStream(data))
            {
                switch (identifier.Type)
                {
                    case IdentifierType.Reference:
                        Guid id = (Guid)formatter.Deserialize(stream);
                        result = mapper.GetObject(identifier.ClassName, id);
                        break;
                    case IdentifierType.Value:
                        result = formatter.Deserialize(stream);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return result;
        }

        public byte[] SerializeDefinitionRequest(Guid definitionId)
        {
            byte[] result = null;
            DefinitionRequest data = new DefinitionRequest();
            data.DefinitionId = definitionId;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.WriteByte((byte)MessageType.MethodDescriptionRequest);
                formatter.Serialize(stream, data);
                result = stream.ToArray();
            }
            return result;
        }

        public byte[] SerializeDefinitionAnswer(Guid definitionId, MethodInfo methodInfo, out Guid id)
        {
            DefinitionAnswer methodCall = new DefinitionAnswer();
            methodCall.DefinitionId = definitionId;
            // Set the this pointer
            Identifier thisObject = new Identifier();
            thisObject.ClassName = methodInfo.DeclaringType.Name;
            methodCall.Target = thisObject;
            // Method name
            methodCall.MethodName = methodInfo.Name;
            // Parameters
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            Identifier[] parameters = new Identifier[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; i++)
            {
                Identifier p = new Identifier();
                p.ClassName = paramInfos[i].ParameterType.FullName;
                parameters[i] = p;
            }
            methodCall.Parameters = parameters;
            // Return type
            Identifier retType = new Identifier();
            retType.ClassName = (methodInfo.ReturnType == null) ? null : methodInfo.ReturnType.FullName;
            //retType.Type = 
            //methodCall.ReturnType = 

            // Serialize
            byte[] result = null;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.WriteByte((byte)MessageType.MethodDescriptionAnswer);
                formatter.Serialize(stream, methodCall);
                result = stream.ToArray();
            }
            id = methodCall.DefinitionId;
            return result;
        }

        public byte[] SerializeCall(Guid definitionId, IObjectMapper mapper, object target, object[] paramValues)
        {
            MethodCall methodCall = new MethodCall();
            methodCall.Id = Guid.NewGuid();
            methodCall.DefinitionId = definitionId;


            string targetClassName = (target is string) ? (string)target : target.GetType().FullName;
            methodCall.Target = Serialize(target, new Identifier(targetClassName, IdentifierType.Reference), mapper);
            methodCall.ParameterValues = new byte[paramValues.Length][];

            for (int i = 0; i < paramValues.Length; i++)
            {
                object value = paramValues[i];
                Type valueType = value.GetType();

                Identifier identifier;
                if (valueType.IsValueType || valueType.IsPrimitive || valueType == typeof(string))
                {
                    identifier.Type = IdentifierType.Value;
                }
                else
                {
                    identifier.Type = IdentifierType.Reference;
                }
                identifier.ClassName = valueType.FullName;
                methodCall.ParameterValues[i] = Serialize(value, identifier, mapper);
            }

            byte[] bytes = null;
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, methodCall);
                bytes = stream.ToArray();
            }
            return bytes;
        }

        public byte[] SerializeCallResponse(Guid id, IObjectMapper mapper, object result)
        {
            MethodResult methodResult = new MethodResult();
            methodResult.Id = id;
            methodResult.Value = Serialize(result);

            byte[] bytes = null;
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, methodResult);
                bytes = stream.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// Desereliaze a message to an expected type
        /// </summary>
        public bool TryDeserialize<T>(byte[] data, int index, int count, out T result)
        {
            bool ok = false;
            result = default(T);
            if (count > 0)
            {
                using (MemoryStream stream = new MemoryStream(data, index, count))
                {
                    MessageType dataType = (MessageType)stream.ReadByte();
                    object dataObject = formatter.Deserialize(stream);
                    if (dataObject is T)
                    {
                        result = (T)dataObject;
                        ok = true;
                    }
                }
            }
            return ok;
        }
    }
}
