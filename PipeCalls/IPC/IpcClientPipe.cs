using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipeCalls.IPC
{
    public class IpcClientPipe
    {
        private readonly NamedPipeClientStream m_pipe;

        public IpcClientPipe(String serverName, String pipename)
        {
            m_pipe = new NamedPipeClientStream(
                serverName,
                pipename,
                PipeDirection.InOut,
                PipeOptions.Asynchronous | PipeOptions.WriteThrough
            );
        }

        public PipeStream Connect(Int32 timeout)
        {
            // NOTE: will throw on failure
            m_pipe.Connect(timeout);

            // Must Connect before setting ReadMode
            m_pipe.ReadMode = PipeTransmissionMode.Message;

            return m_pipe;
        }
    }
}
