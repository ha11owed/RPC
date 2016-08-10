using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    class MessageType
    {
        private delegate void SerializeFieldDelegate(AMessage message, BytesBuffer byteBuffer);
        private delegate void DeserializeFieldDelegate(AMessage message, BytesBuffer byteBuffer);

        private readonly Type messageType;
        private readonly SerializeFieldDelegate[] serializers;
        private readonly DeserializeFieldDelegate[] deserializers;

        public MessageType(Type messageType)
        {
            this.messageType = messageType;

            List<KeyValuePair<int, PropertyInfo>> properties = new List<KeyValuePair<int, PropertyInfo>>();
            foreach (PropertyInfo pi in messageType.GetProperties())
            {
                MessageFieldAttribute fieldAttribute = pi.GetCustomAttribute(typeof(MessageFieldAttribute)) as MessageFieldAttribute;
                if (fieldAttribute != null)
                {
                    properties.Add(new KeyValuePair<int, PropertyInfo>(fieldAttribute.Index, pi));
                }
            }
            properties = properties.OrderBy(p => p.Key).ToList();
            for (int i = 0; i < properties.Count - 1; i++)
            {
                if (properties[i].Key == properties[i + 1].Key)
                    throw new ArgumentException(string.Format("Duplicate Field index: {0} found on type: {1}", properties[i].Key, messageType));
            }

            serializers = new SerializeFieldDelegate[properties.Count];
            deserializers = new DeserializeFieldDelegate[properties.Count];

            for (int i = 0; i < properties.Count; i++)
            {
                int index = properties[i].Key;
                PropertyInfo propertyInfo = properties[i].Value;

                SerializeFieldDelegate sf = null;
                DeserializeFieldDelegate df = null;

                if (propertyInfo.PropertyType == typeof(int))
                {
                    Func<AMessage, int> getter = CreateMethod<Func<AMessage, int>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteInt(getter(m));

                    Action<AMessage, int> setter = CreateMethod<Action<AMessage, int>>(propertyInfo.GetSetMethod());
                    df = (m, bb) => setter(m, bb.ReadInt32());
                }
                else if (propertyInfo.PropertyType == typeof(uint))
                {
                    Func<AMessage, uint> getter = CreateMethod<Func<AMessage, uint>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteUInt(getter(m));

                    Action<AMessage, uint> setter = CreateMethod<Action<AMessage, uint>>(propertyInfo.GetSetMethod());
                    df = (m, bb) => setter(m, bb.ReadUInt32());
                }
                else if (propertyInfo.PropertyType == typeof(Guid))
                {
                    Func<AMessage, Guid> getter = CreateMethod<Func<AMessage, Guid>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteGuid(getter(m));

                    Action<AMessage, Guid> setter = CreateMethod<Action<AMessage, Guid>>(propertyInfo.GetSetMethod());
                    df = (m, bb) => setter(m, bb.ReadGuid());
                }
                else if (propertyInfo.PropertyType == typeof(object))
                {
                    Func<AMessage, object> getter = CreateMethod<Func<AMessage, object>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteObject(getter(m));

                    Action<AMessage, object> setter = CreateMethod<Action<AMessage, object>>(propertyInfo.GetSetMethod());
                    df = (m, bb) => setter(m, bb.ReadObject());
                }
                else if (propertyInfo.PropertyType == typeof(object[]))
                {
                    Func<AMessage, object[]> getter = CreateMethod<Func<AMessage, object[]>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteObjectArray(getter(m));

                    Action<AMessage, object[]> setter = CreateMethod<Action<AMessage, object[]>>(propertyInfo.GetSetMethod());
                    df = (m, bb) => setter(m, bb.ReadObjectArray());
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    Func<AMessage, string> getter = CreateMethod<Func<AMessage, string>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteString(getter(m));

                    Action<AMessage, string> setter = CreateMethod<Action<AMessage, string>>(propertyInfo.GetSetMethod());
                    df = (m, bb) => setter(m, bb.ReadString());
                }
                else if (propertyInfo.PropertyType == typeof(Type))
                {
                    Func<AMessage, Type> getter = CreateMethod<Func<AMessage, Type>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteType(getter(m));

                    Action<AMessage, Type> setter = CreateMethod<Action<AMessage, Type>>(propertyInfo.GetSetMethod());
                    df = (m, bb) => setter(m, bb.ReadType());
                }
                else if (propertyInfo.PropertyType == typeof(MethodInfo))
                {
                    Func<AMessage, MethodInfo> getter = CreateMethod<Func<AMessage, MethodInfo>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteMethodInfo(getter(m));

                    Action<AMessage, MethodInfo> setter = CreateMethod<Action<AMessage, MethodInfo>>(propertyInfo.GetSetMethod());
                    df = (m, bb) => setter(m, bb.ReadMethodInfo());
                }
                else
                {
                    throw new NotImplementedException("Type " + propertyInfo.PropertyType + " is not suppoerted");
                }
                serializers[i] = sf;
                deserializers[i] = df;
            }
        }

        public Type MessageClassType { get { return messageType; } }

        public void Serialize(AMessage message, BytesBuffer bb)
        {
            foreach (var serializer in serializers)
            {
                serializer(message, bb);
            }
        }

        public void Deserialize(AMessage message, byte[] data)
        {
            Deserialize(message, new BytesBuffer(data));
        }

        public void Deserialize(AMessage message, BytesBuffer bb)
        {
            foreach (var deserializer in deserializers)
            {
                deserializer(message, bb);
            }
        }

        /// <summary>
        /// Creates a proxy to a public method belonging to a class.
        /// The first parameter of the method will be the instance of the object we are calling it on.
        /// </summary>
        public static TDelegate CreateMethod<TDelegate>(MethodInfo methodInfo)
        {
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            ParameterExpression[] parameterExpressionsWithThis = new ParameterExpression[parameterInfos.Length + 1];
            ParameterExpression[] parameterExpressions = new ParameterExpression[parameterInfos.Length];
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                ParameterInfo pi = parameterInfos[i];
                ParameterExpression pe = Expression.Parameter(pi.ParameterType, pi.Name);
                parameterExpressions[i] = pe;
            }
            Array.Copy(parameterExpressions, 0, parameterExpressionsWithThis, 1, parameterExpressions.Length);
            parameterExpressionsWithThis[0] = Expression.Parameter(typeof(AMessage), "oThis");

            Type targetType = methodInfo.DeclaringType;

            Expression callExpression = Expression.Call(
                Expression.Convert(parameterExpressionsWithThis[0], targetType),
                methodInfo,
                parameterExpressions);

            Expression<TDelegate> methodExpression = Expression.Lambda<TDelegate>(callExpression, parameterExpressionsWithThis);
            TDelegate method = methodExpression.Compile();
            return method;
        }
    }
}
