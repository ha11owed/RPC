using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    public class StreamHelper
    {
        public static AsyncResult BeginWrite(Stream stream, AMessage message)
        {
            AsyncResult ar = new AsyncResult();
            MessageWriteState state = new MessageWriteState(stream, message, ar);
            stream.BeginWrite(state.Buffer, 0, state.Buffer.Length, OnBufferWrite, state);
            return ar;
        }

        private static void OnBufferWrite(IAsyncResult ar)
        {
            MessageWriteState state = (MessageWriteState)ar.AsyncState;
            state.Stream.EndWrite(ar);
            state.Finish();
        }

        public static AsyncResult BeginRead(Stream stream)
        {
            AsyncResult ar = new AsyncResult();
            MessageReadState state = new MessageReadState(stream, ar);
            stream.BeginRead(state.Header, 0, state.Header.Length, OnHeaderRead, state);
            return ar;
        }

        private static void OnHeaderRead(IAsyncResult ar)
        {
            MessageReadState state = (MessageReadState)ar.AsyncState;
            int n = state.Stream.EndRead(ar);
            state.HeaderLength += n;

            int index = state.HeaderLength;
            int length = state.Header.Length;

            if (index == length)
            {
                // Allocate the body. We are done reading the header
                byte[] data = new byte[state.GetDataLength()];
                state.Data = data;
                state.Stream.BeginRead(data, 0, data.Length, OnDataRead, state);
            }
            else if (n > 0)
            {
                // Read the rest of the header
                state.Stream.BeginRead(state.Header, index, length - index, OnHeaderRead, state);
            }
            else
            {
                // The stream has ended, but we have not finished reading the header
                state.Finish();
            }
        }

        private static void OnDataRead(IAsyncResult ar)
        {
            MessageReadState state = (MessageReadState)ar.AsyncState;
            int n = state.Stream.EndRead(ar);
            state.DataLength += n;

            int index = state.DataLength;
            int length = state.Data.Length;

            if (index == length)
            {
                // We are done reading
                state.Finish();
            }
            else if (n > 0)
            {
                // We still have body data to read
                state.Stream.BeginRead(state.Data, index, length - index, OnDataRead, state);
            }
            else
            {
                // The read is incomplete
                state.Finish();
            }
        }
    }
}
