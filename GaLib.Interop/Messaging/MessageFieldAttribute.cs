using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Messaging
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    class MessageFieldAttribute : Attribute
    {
        private readonly int index;

        public MessageFieldAttribute(int index)
        {
            this.index = index;
        }

        public int Index { get { return index; } }
    }
}
