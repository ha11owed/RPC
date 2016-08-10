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
    abstract class BytesBufferBase
    {
        private const int N = 32;

        private byte[] buffer = null;
        private int index = 0;
        private int length = 0;

        public BytesBufferBase(byte[] data)
        {
            this.buffer = data;
            length = data.Length;
        }

        public BytesBufferBase(int initialCapacity)
        {
            this.buffer = new byte[initialCapacity];
        }

        public BytesBufferBase()
        {
        }

        /// <summary>
        /// The number of bytes available for reading
        /// </summary>
        public int AvailableBytes { get { return length - index; } }

        /// <summary>
        /// Consolidate the buffer. Eat up the consumed bytes.
        /// Do this after consuming the content in order to prevent allocating more memory then needed.
        /// </summary>
        public void Consolidate()
        {
            if (length > 0)
            {
                Array.Copy(buffer, index, buffer, 0, length);
            }
            index = 0;
        }
        
        /// <summary>
        /// Get the data as a byte array
        /// </summary>
        public byte[] ToByteArray()
        {
            if (index == length)
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

        /// <summary>
        /// Write a number of bytes from the long value into the buffer
        /// </summary>
        public void WriteBytes(long value, int bytesCount)
        {
            if (bytesCount > sizeof(long)) { throw new ArgumentException("The maximum number of bytes that fit in a long are: " + sizeof(long)); }
            EnsureBuffer(bytesCount);

            int endOffset = index + bytesCount - 1;
            for (int i = 0; i < bytesCount; i++)
            {
                buffer[endOffset - i] = unchecked((byte)(value & 0xff));
                value = value >> 8;
            }
            index += bytesCount;
            length += bytesCount;
        }

        /// <summary>
        /// Write data to the buffer
        /// </summary>
        /// <param name="bytes">The buffer containing the data</param>
        /// <param name="offset">The offset in the buffer where the data starts</param>
        /// <param name="count">The number of bytes to write starting at the offset</param>
        public void WriteBytes(byte[] bytes, int offset, int count)
        {
            EnsureBuffer(count);
            Array.Copy(bytes, offset, buffer, index, count);
            index += count;
            length += count;
        }

        /// <summary>
        /// Read a number of bytes and store it into a long
        /// </summary>
        public long ReadBytes(int bytesCount)
        {
            if (bytesCount > sizeof(long)) { throw new ArgumentException("The maximum number of bytes that fit in a long are: " + sizeof(long)); }
            if (index + bytesCount > length) { throw new ArgumentOutOfRangeException(string.Format("Cannot read {0} bytes as long", bytesCount)); }

            long ret = 0;
            for (int i = 0; i < bytesCount; i++)
            {
                ret = unchecked((ret << 8) | buffer[index + i]);
            }
            index += bytesCount;
            return ret;
        }

        /// <summary>
        /// Read data into the given byte array
        /// </summary>
        /// <param name="data">The buffer to put the read data into</param>
        /// <param name="offset">The position in the buffer where we start putting data</param>
        /// <param name="count">The number of bytes to read</param>
        public void ReadBytes(byte[] data, int offset, int count)
        {
            if (index + count > length) { throw new ArgumentOutOfRangeException(string.Format("Cannot read {0} bytes in the out buffer", count)); }
            Array.Copy(buffer, index, data, offset, count);
            index += count;
        }

        /// <summary>
        /// Read a single byte from the buffer
        /// </summary>
        public byte ReadByte()
        {
            return unchecked((byte)ReadBytes(1));
        }

        /// <summary>
        /// Read a 32-bit signed integer from the buffer.
        /// </summary>
        public int ReadInt32()
        {
            return unchecked((int)(ReadBytes(4)));
        }

        /// <summary>
        /// Read a 32-bit unsigned integer from the buffer.
        /// </summary>
        public uint ReadUInt32()
        {
            return unchecked((uint)(ReadBytes(4)));
        }

        /// <summary>
        /// Read a string from the buffer
        /// </summary>
        public string ReadString()
        {
            int len = ReadInt32();
            if (len < 0)
                return null;
            if (len == 0)
                return "";

            if (index + len > length) { throw new IndexOutOfRangeException("The buffer does not contain enough data in order to read the string"); }
            string str = Encoding.UTF8.GetString(buffer, index, len);
            index += len;
            return str;
        }

        /// <summary>
        /// Read a GUID from the buffer
        /// </summary>
        public Guid ReadGuid()
        {
            byte[] guidBuffer = new byte[16];
            ReadBytes(guidBuffer, 0, guidBuffer.Length);
            Guid result = new Guid(guidBuffer);
            return result;
        }

        /// <summary>
        /// Write a signed int to the buffer
        /// </summary>
        public void WriteInt(Int32 value)
        {
            WriteBytes(value, 4);
        }

        /// <summary>
        /// Write an unsigned int to the buffer
        /// </summary>
        public void WriteUInt(UInt32 value)
        {
            WriteBytes(value, 4);
        }

        /// <summary>
        /// Write a GUID to the buffer
        /// </summary>
        public void WriteGuid(Guid value)
        {
            byte[] bytes = value.ToByteArray();
            WriteBytes(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Write a string to the buffer
        /// </summary>
        public void WriteString(string value)
        {
            if (value == null)
            {
                WriteBytes(-1, 4);
            }
            else if (value.Length == 0)
            {
                WriteBytes(0, 4);
            }
            else
            {
                byte[] bytes = Encoding.UTF8.GetBytes(value);
                WriteBytes(bytes.Length, 4);
                WriteBytes(bytes, 0, bytes.Length);
            }
        }
    }
}
