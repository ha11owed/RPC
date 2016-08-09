﻿using GaLib.Interop.Protocol.Messages;
using GaLib.Interop.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Protocol
{
    class Definition
    {
        private readonly DefinitionAnswer definitionAnswer;

        private readonly Type targetType;
        private readonly Type[] paramTypes;
        private readonly Type returnType;

        private readonly MethodInfo methodInfo;

        public Definition(DefinitionAnswer definitionAnswer)
        {
            this.definitionAnswer = definitionAnswer;

            // Target type
            this.targetType = Type.GetType(definitionAnswer.Target.ClassName);

            // Parameters
            int n = (definitionAnswer.Parameters == null) ? 0 : definitionAnswer.Parameters.Length;
            this.paramTypes = new Type[n];
            for (int i = 0; i < n; i++)
            {
                this.paramTypes[i] = Type.GetType(definitionAnswer.Parameters[i].ClassName);
            }

            // Return type
            if (definitionAnswer.ReturnType.ClassName != null)
                this.returnType = Type.GetType(definitionAnswer.ReturnType.ClassName);
            else
                this.returnType = null;

            // Identify the method/property/event
            foreach (MethodInfo methodInfo in this.targetType.GetMethods())
            {
                if (methodInfo.Name != definitionAnswer.MethodName)
                    continue;

                if (!object.Equals(methodInfo.ReturnType, this.returnType))
                    continue;

                ParameterInfo[] paramInfos = methodInfo.GetParameters();
                if (paramInfos.Length != paramTypes.Length)
                    continue;

                bool sameType = true;
                for (int i = 0; i < paramInfos.Length; i++)
                {
                    Type found = paramInfos[i].ParameterType;
                    Type actual = paramTypes[i];
                    if (!found.Equals(actual))
                    {
                        sameType = false;
                        break;
                    }
                }
                if (!sameType)
                    break;

                this.methodInfo = methodInfo;
                break;
            }
        }

        public byte[] Call(Call call, IObjectMapper mapper, BytesBuffer serializer)
        {
            object result = null;
            object target = null;
            if (call.Target != null)
            {
                //target = serializer.Deserialize(call.Target, definitionAnswer.Target, mapper);
            }

            int nParameters = (definitionAnswer.Parameters == null) ? 0 : definitionAnswer.Parameters.Length;
            object[] parameters = new object[nParameters];
            for (int i = 0; i < parameters.Length; i++)
            {
                Identifier identifier = definitionAnswer.Parameters[i];
                //byte[] paramValue = call.ParameterValues[i];
                //parameters[i] = serializer.Deserialize(paramValue, identifier, mapper);
            }

            result = this.methodInfo.Invoke(target, parameters);

            byte[] resultBytes = null;
            if (result != null)
            {
                //resultBytes = serializer.Serialize(result, definitionAnswer.ReturnType, mapper);
            }
            return resultBytes;
        }
    }
}