using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    class MessageReader
    {
        private const int HeaderSize = 1 + sizeof(int);

        enum ReadState
        {
            Header,
            Data,
        }

        private BytesBuffer buffer = new BytesBuffer(1024);
        private ReadState state = ReadState.Header;

        private byte messageId;
        private int dataLength;

        public AMessage Process(byte[] data, int offset, int count)
        {
            AMessage message = null;
            buffer.WriteBytes(data, offset, count);
            switch (state)
            {
                case ReadState.Header:
                    if (buffer.AvailableBytes >= HeaderSize)
                    {
                        messageId = buffer.ReadByte();
                        dataLength = buffer.ReadInt32();
                        state = ReadState.Data;
                    }
                    break;
                case ReadState.Data:
                    if (buffer.AvailableBytes >= dataLength)
                    {
                        message = MessageManager.Create(messageId, buffer);
                        buffer.Consolidate();
                        state = ReadState.Header;
                    }
                    break;
            }
            return message;
        }
    }
}
