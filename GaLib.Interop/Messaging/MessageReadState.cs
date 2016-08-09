using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    class MessageReadState
    {
        private readonly Stream stream;
        private readonly byte[] header = new byte[sizeof(int) + 1];
        private readonly AsyncResult asyncResult;

        public MessageReadState(Stream stream, AsyncResult asyncResult)
        {
            this.stream = stream;
            this.asyncResult = asyncResult;
        }

        public Stream Stream { get { return stream; } }
        public byte[] Header { get { return header; } }
        public byte[] Data { get; set; }

        public int HeaderLength { get; set; }
        public int DataLength { get; set; }

        public byte MessageId { get { return header[0]; } }

        public int GetDataLength()
        {
            return (int)BytesBuffer.FromBytes(4, header, 1);
        }

        public void Finish()
        {
            if (HeaderLength == Header.Length
                && Data != null
                && DataLength == Data.Length)
            {
                // Create the message
                if (asyncResult != null)
                {
                    AMessage message = MessageManager.Create(MessageId, Data);
                    asyncResult.AsyncState = message;
                    asyncResult.Complete();
                }
            }
        }
    }
}
