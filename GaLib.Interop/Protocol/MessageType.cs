using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Protocol
{
    class MessageType
    {
        private delegate void SerializeFieldDelegate(AMessage message, BytesBuffer byteBuffer);

        private readonly Type messageType;
        private readonly SerializeFieldDelegate[] serializers;

        public MessageType(Type messageType)
        {
            this.messageType = messageType;

            List<KeyValuePair<int, PropertyInfo>> properties = new List<KeyValuePair<int, PropertyInfo>>();
            foreach (PropertyInfo pi in messageType.GetProperties())
            {
                FieldAttribute fieldAttribute = pi.GetCustomAttribute(typeof(FieldAttribute)) as FieldAttribute;
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
            for (int i = 0; i < properties.Count; i++)
            {
                int index = properties[i].Key;
                PropertyInfo propertyInfo = properties[i].Value;
                SerializeFieldDelegate sf = null;
                if (propertyInfo.PropertyType == typeof(int))
                {
                    Func<AMessage, int> getter = CreateMethod<Func<AMessage, int>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteInt(getter(m));
                }
                else if (propertyInfo.PropertyType == typeof(uint))
                {
                    Func<AMessage, uint> getter = CreateMethod<Func<AMessage, uint>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteUInt(getter(m));
                }
                else if (propertyInfo.PropertyType == typeof(Guid))
                {
                    Func<AMessage, Guid> getter = CreateMethod<Func<AMessage, Guid>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteGuid(getter(m));
                }
                else if (propertyInfo.PropertyType == typeof(object))
                {
                    Func<AMessage, object> getter = CreateMethod<Func<AMessage, object>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteObject(getter(m));
                }
                else if (propertyInfo.PropertyType == typeof(object[]))
                {
                    Func<AMessage, object[]> getter = CreateMethod<Func<AMessage, object[]>>(propertyInfo.GetGetMethod());
                    sf = (m, bb) => bb.WriteObjectArray(getter(m));
                }
                serializers[i] = sf;
            }
        }

        public byte[] Compile(AMessage message)
        {
            BytesBuffer bb = new BytesBuffer();
            foreach (var serializer in serializers)
            {
                serializer(message, bb);
            }
            return bb.ToByteArray();
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
