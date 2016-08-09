using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Protocol
{
    abstract class AMessage : IMessage
    {
        private static readonly ConcurrentDictionary<Type, MessageType> messageTypes = new ConcurrentDictionary<Type, MessageType>();

        private readonly MessageType messageType;
        private byte[] data;

        public AMessage()
        {
            messageType = messageTypes.GetOrAdd(GetType(), (t) => new MessageType(t));
        }

        /// <summary>
        /// The Id of the message
        /// </summary>
        public abstract MessageId Id { get; }

        public byte[] GetBytes()
        {
            return data;
        }

        public byte[] Compile()
        {
            data = messageType.Compile(this);
            return data;
        }
    }
}
