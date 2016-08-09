using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    class BytesBuffer
    {
        private const int N = 32;

        public BytesBuffer(byte[] data)
        {
            this.buffer = data;
        }

        public BytesBuffer()
        {
        }

        public byte[] ToByteArray()
        {
            if (index == buffer.Length)
                return buffer;
            else
                return buffer.Take(index).ToArray();
        }

        private void EnsureBuffer(int bytesCount)
        {
            if (buffer == null)
            {
                int n = ((bytesCount + N - 1) / N) * N;
                buffer = new byte[n];
            }
            else if (buffer.Length < index + bytesCount)
            {
                // We need to resize the buffer
                int n = ((bytesCount + N - 1) / N) * N;
                Array.Resize(ref buffer, buffer.Length + n);
            }
        }

        private void ToBytes(long value, int bytesCount)
        {
            EnsureBuffer(bytesCount);

            int endOffset = index + bytesCount - 1;
            for (int i = 0; i < bytesCount; i++)
            {
                buffer[endOffset - i] = unchecked((byte)(value & 0xff));
                value = value >> 8;
            }
            index += bytesCount;
        }

        private long FromBytes(int bytesCount)
        {
            long ret = 0;
            for (int i = 0; i < bytesCount; i++)
            {
                ret = unchecked((ret << 8) | buffer[index + i]);
            }
            index += bytesCount;
            return ret;
        }

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

        private byte[] buffer = null;
        private int index = 0;

        /// <summary>
        /// Returns a 32-bit signed integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        public int ReadInt32()
        {
            return unchecked((int)(FromBytes(4)));
        }

        /// <summary>
        /// Returns a 32-bit unsigned integer converted from four bytes at a specified position in a byte array.
        /// </summary>
        public uint ReadUInt32()
        {
            return unchecked((uint)(FromBytes(4)));
        }

        public string ReadString()
        {
            int len = ReadInt32();
            if (len < 0)
                return null;
            if (len == 0)
                return "";

            string str = Encoding.UTF8.GetString(buffer, index, len);
            index += len;
            return str;
        }

        public Guid ReadGuid()
        {
            byte[] guidBuffer = new byte[16];
            Array.Copy(buffer, index, guidBuffer, 0, guidBuffer.Length);
            Guid result = new Guid(guidBuffer);
            index += guidBuffer.Length;
            return result;
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="buffer">The buffer to copy the bytes to</param>
        /// <param name="index">The index where to start copying</param>
        public void WriteInt(int value)
        {
            ToBytes(value, 4);
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="buffer">The buffer to copy the bytes to</param>
        /// <param name="index">The index where to start copying</param>
        public void WriteUInt(uint value)
        {
            ToBytes(value, 4);
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        /// <param name="value">The value to convert</param>
        public void WriteGuid(Guid value)
        {
            byte[] bytes = value.ToByteArray();
            EnsureBuffer(bytes.Length);
            Array.Copy(bytes, 0, buffer, index, bytes.Length);
            index += bytes.Length;
        }

        /// <summary>
        /// Convert the value to bytes and copies it into the buffer
        /// </summary>
        /// <param name="value">The value to convert</param>
        public void WriteString(string value)
        {
            if (value == null)
            {
                ToBytes(-1, 4);
            }
            else if (value.Length == 0)
            {
                ToBytes(0, 4);
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                ToBytes(bytes.Length, 4);
                EnsureBuffer(bytes.Length);
                Array.Copy(bytes, 0, buffer, index, bytes.Length);
                index += bytes.Length;
            }
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
                ToBytes(0, 1);
            }
            else if (value is Int32)
            {
                ToBytes(1, 1);
                WriteInt((Int32)value);
            }
            else if (value is UInt32)
            {
                ToBytes(2, 1);
                WriteUInt((UInt32)value);
            }
            else if (value is Guid)
            {
                ToBytes(19, 1);
                WriteGuid((Guid)value);
            }
            else if (value is string)
            {
                ToBytes(20, 1);
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
            long id = FromBytes(1);
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
