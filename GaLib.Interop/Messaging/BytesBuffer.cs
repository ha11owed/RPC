using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    class BytesBuffer : BytesBufferBase
    {
        public static void ToBytes(long value, int bytesCount, byte[] buffer, int index)
        {
            int endOffset = index + bytesCount - 1;
            for (int i = 0; i < bytesCount; i++)
            {
                buffer[endOffset - i] = unchecked((byte)(value & 0xff));
                value = value >> 8;
            }
            index += bytesCount;
        }

        public static long FromBytes(int bytesCount, byte[] buffer, int index)
        {
            long ret = 0;
            for (int i = 0; i < bytesCount; i++)
            {
                ret = unchecked((ret << 8) | buffer[index + i]);
            }
            index += bytesCount;
            return ret;
        }

        public BytesBuffer(int initialCapacity)
            : base(initialCapacity)
        {
        }

        public BytesBuffer(byte[] data)
            : base(data)
        {
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        /// <param name="value">The value to convert</param>
        public void WriteType(Type value)
        {
            if (value == null)
            {
                WriteString(null);
            }
            else
            {
                WriteString(value.AssemblyQualifiedName);
            }
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        public Type ReadType()
        {
            string fullName = ReadString();
            Type result = null;
            if (fullName != null)
            {
                result = Type.GetType(fullName);
            }
            return result;
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        /// <param name="value">The value to convert</param>
        public void WriteMethodInfo(MethodInfo value)
        {
            if (value == null)
            {
                WriteString(null);
            }
            else
            {
                WriteString(value.Name);
                WriteType(value.DeclaringType);
                WriteType(value.ReturnType);
                ParameterInfo[] paramInfo = value.GetParameters();
                WriteInt(paramInfo.Length);
                foreach (ParameterInfo pi in paramInfo)
                {
                    WriteType(pi.ParameterType);
                }
            }
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        public MethodInfo ReadMethodInfo()
        {
            MethodInfo result = null;
            string name = ReadString();
            if (name != null)
            {
                Type declaringType = ReadType();
                Type returnType = ReadType();
                int paramCount = ReadInt32();
                Type[] paramTypes = new Type[paramCount];
                for (int i = 0; i < paramCount; i++)
                {
                    paramTypes[0] = ReadType();
                }

                foreach (MethodInfo mi in declaringType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (mi.Name != name)
                        continue;
                    ParameterInfo[] parameterInfos = mi.GetParameters();
                    if (parameterInfos.Length != paramCount)
                        continue;

                    if (mi.ReturnType != returnType)
                        continue;

                    bool ok = true;
                    for (int i = 0; i < paramCount; i++)
                    {
                        if (parameterInfos[i].ParameterType != paramTypes[i])
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (!ok)
                        break;

                    result = mi;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        /// <param name="value">The value to convert</param>
        public void WriteObject(object value)
        {
            if (value == null)
            {
                WriteBytes(0, 1);
            }
            else if (value is Int32)
            {
                WriteBytes(1, 1);
                WriteInt((Int32)value);
            }
            else if (value is UInt32)
            {
                WriteBytes(2, 1);
                WriteUInt((UInt32)value);
            }
            else if (value is Guid)
            {
                WriteBytes(19, 1);
                WriteGuid((Guid)value);
            }
            else if (value is string)
            {
                WriteBytes(20, 1);
                WriteString((string)value);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        /// <param name="value">The value to convert</param>
        public object ReadObject()
        {
            long id = ReadBytes(1);
            if (id == 0)
            {
                return null;
            }
            else if (id == 1)
            {
                return ReadInt32();
            }
            else if (id == 2)
            {
                return ReadUInt32();
            }
            else if (id == 19)
            {
                return ReadGuid();
            }
            else if (id == 20)
            {
                return ReadString();
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        /// <param name="value">The value to convert</param>
        public void WriteObjectArray(object[] value)
        {
            if (value == null)
            {
                WriteInt(-1);
            }
            else if (value.Length == 0)
            {
                WriteInt(0);
            }
            else
            {
                WriteInt(value.Length);
                foreach (object o in value)
                {
                    WriteObject(o);
                }
            }
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        public object[] ReadObjectArray()
        {
            int len = ReadInt32();
            if (len == -1)
                return null;
            if (len == 0)
                return new object[0];

            object[] result = new object[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = ReadObject();
            }
            return result;
        }
    }
}
