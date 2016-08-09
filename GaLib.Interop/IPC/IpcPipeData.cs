using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.IPC
{
    /// <summary>
    /// Internal data associated with pipes
    /// </summary>
    struct IpcPipeData
    {
        public PipeStream pipe;
        public Object state;
        public Byte[] data;
    };
}
