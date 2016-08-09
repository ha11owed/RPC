using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    class MessageWriteState
    {
        private readonly Stream stream;

        private readonly byte[] buffer;

        private readonly AMessage message;
        private readonly AsyncResult asyncResult;

        public MessageWriteState(Stream stream, AMessage message, AsyncResult asyncResult)
        {
            this.stream = stream;
            this.message = message;
            this.asyncResult = asyncResult;

            byte[] data = message.Compile();
            int headerSize = 1 + sizeof(int);
            buffer = new byte[headerSize + data.Length];
            // Populate the header
            buffer[0] = message.Id;
            BytesBuffer.ToBytes(data.Length, sizeof(int), buffer, 1);
            // Populate the data
            Array.Copy(data, 0, buffer, headerSize, data.Length);
        }

        public Stream Stream { get { return stream; } }

        public byte[] Buffer { get { return buffer; } }

        public void Finish()
        {
            // Create the message
            if (asyncResult != null)
            {
                asyncResult.AsyncState = message;
                asyncResult.Complete();
            }
        }
    }
}
