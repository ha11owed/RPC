using PipeCalls.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.Internal
{
    /// <summary>
    /// The state of the current connection
    /// </summary>
    class ConnectionState
    {
        private readonly int connectionIndex;
        private readonly Serializer serialization = new Serializer();
        private readonly Queue<MethodCall> queue = new Queue<MethodCall>();

        public ConnectionState(int connectionIndex)
        {
            this.connectionIndex = connectionIndex;
        }

        /// <summary>
        /// The queue of with the calls
        /// </summary>
        public Queue<MethodCall> Queue { get { return queue; } }

        /// <summary>
        /// Object used for serialization
        /// </summary>
        public Serializer Serialization { get { return serialization; } }
        
        /// <summary>
        /// The number of the connection
        /// </summary>
        public int ConnectionIndex { get { return connectionIndex; } }

        /// <summary>
        /// String representation of the connection state
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("ConnectionState[Index=");
            sb.Append(ConnectionIndex);
            sb.Append("]");

            string result = sb.ToString();
            return result;
        }
    }
}
