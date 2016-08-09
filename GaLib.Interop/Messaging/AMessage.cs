using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    /// <summary>
    /// Base class for all the messages used to do the IPC communication
    /// </summary>
    public abstract class AMessage
    {
        private readonly MessageType messageType;

        protected AMessage()
        {
            messageType = MessageManager.GetMessageType(GetType());
        }

        /// <summary>
        /// The Id of the message
        /// </summary>
        public abstract byte Id { get; }

        public byte[] Compile()
        {
            byte[] data = messageType.Compile(this);
            return data;
        }

        public AsyncResult BeginWrite(Stream stream)
        {
            return StreamHelper.BeginWrite(stream, this);
        }

        public static AsyncResult BeginRead(Stream stream)
        {
            return StreamHelper.BeginRead(stream);
        }

        public static AMessage Create(byte id, byte[] data)
        {
            return MessageManager.Create(id, data);
        }
    }
}
