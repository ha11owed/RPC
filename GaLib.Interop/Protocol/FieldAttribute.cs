﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Protocol
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    class FieldAttribute : Attribute
    {
        private readonly int index;

        public FieldAttribute(int index)
        {
            this.index = index;
        }

        public int Index { get { return index; } }
    }
}
