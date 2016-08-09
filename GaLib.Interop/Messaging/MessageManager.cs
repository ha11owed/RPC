using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    class MessageManager
    {
        /// <summary>
        /// Scan the assemblies for the message types
        /// </summary>
        private static MessageType[] ScanAssemblies()
        {
            MessageType[] messages = new MessageType[256];

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                string fullName = assembly.FullName;
                if (fullName.StartsWith("System.") || fullName.StartsWith("Microsoft."))
                {
                    continue;
                }

                Type[] types = null;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // The DLL might have some types that depend on other DLLs.
                    // If those DLLs are missing and exception is thrown,
                    // but we can still retrieve a list of the loaded types.
                    types = ex.Types;
                }

                foreach (Type type in types)
                {
                    if (type == null)
                        continue;

                    if (!type.IsSubclassOf(typeof(AMessage)))
                        continue;

                    if (type.IsAbstract)
                        continue;

                    ConstructorInfo constructorInfo = type.GetConstructor(new Type[0]);
                    if (constructorInfo == null || !constructorInfo.IsPublic)
                        continue;

                    AMessage message = (AMessage)constructorInfo.Invoke(new object[0]);
                    byte id = message.Id;

                    if (messages[id] != null)
                        throw new InvalidOperationException("A message with the Id: " + id + " already exists");

                    MessageType messageType = new MessageType(type);
                    messages[id] = messageType;
                }
            }
            return messages;
        }

        private static readonly MessageType[] messageTypes;
        private static readonly Dictionary<Type, MessageType> typeToMessageType = new Dictionary<Type, MessageType>();

        static MessageManager()
        {
            messageTypes = ScanAssemblies();
            foreach (MessageType mt in messageTypes)
            {
                if (mt == null)
                    continue;

                typeToMessageType[mt.MessageClassType] = mt;
            }
        }

        public static MessageType GetMessageType(Type messageType)
        {
            MessageType mt;
            typeToMessageType.TryGetValue(messageType, out mt);
            return mt;
        }

        public static AMessage Create(byte id, byte[] data)
        {
            AMessage result = null;
            MessageType messageType = messageTypes[id];
            if (messageType != null)
            {
                result = (AMessage)Activator.CreateInstance(messageType.MessageClassType);
                messageType.Deserialize(result, data);
            }
            return result;
        }
    }
}
