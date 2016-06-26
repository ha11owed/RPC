using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.IPC
{
    /// <summary>
    /// Interface for user code to receive notifications regarding pipe messages
    /// </summary>
    public interface IpcCallback
    {
        void OnAsyncConnect(PipeStream pipe, out Object state);
        void OnAsyncDisconnect(PipeStream pipe, Object state);
        void OnAsyncMessage(PipeStream pipe, Byte[] data, Int32 bytes, Object state);
    }
}
