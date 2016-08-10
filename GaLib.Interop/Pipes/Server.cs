using GaLib.Interop.IPC;
using GaLib.Interop.Messaging;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaLib.Interop.Pipes
{
    public sealed class Server : IpcCallback, IDisposable
    {
        private readonly object sync = new object();
        private readonly string token;

        private IpcServer ipcServer = null;

        public Server(string token)
        {
            this.token = token;
        }

        public void Start()
        {
            lock (sync)
            {
                Stop();
                ipcServer = new IpcServer(token, this, 10);
            }
        }

        public void Stop()
        {
            lock (sync)
            {
                if (ipcServer != null)
                    ipcServer.IpcServerStop();

                ipcServer = null;
            }
        }

        public void Dispose()
        {
            Stop();
        }

        void IpcCallback.OnAsyncConnect(PipeStream pipe, out object state)
        {
            state = new ServerClient();
        }

        void IpcCallback.OnAsyncDisconnect(PipeStream pipe, object state)
        {
            ServerClient client = (ServerClient)state;
        }

        void IpcCallback.OnAsyncMessage(PipeStream pipe, byte[] data, int bytes, object state)
        {
            ServerClient client = (ServerClient)state;
            AMessage message = client.Reader.Process(data, 0, bytes);
            if (message != null)
            {
            }

            // Write answer to client
            try
            {
                pipe.BeginWrite(data, 0, bytes, OnAsyncWriteComplete, pipe);
            }
            catch (Exception)
            {
                pipe.Close();
            }
        }

        private void OnAsyncWriteComplete(IAsyncResult result)
        {
            PipeStream pipe = (PipeStream)result.AsyncState;
            pipe.EndWrite(result);
        }
    }
}
