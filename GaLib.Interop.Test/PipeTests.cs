using GaLib.Interop.IPC;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GaLib.Interop.Test
{
    sealed class PushAndPullServer : IpcCallback, IDisposable
    {
        class PushAndPullClient
        {
            public PipeStream Stream { get; set; }
            public int Id { get; set; }
        }

        private readonly IpcServer server;
        private int clientId = 0;

        public PushAndPullServer(string name)
        {
            server = new IpcServer(name, this, 10);
        }

        private static void PeriodicPush(PushAndPullClient client)
        {
            int i = 0;
            try
            {
                while (client.Stream.IsConnected)
                {
                    i++;
                    Thread.Sleep(1000);
                    string dataStr = string.Format("{0,5}", i);
                    byte[] data = Encoding.ASCII.GetBytes(dataStr);
                    client.Stream.BeginWrite(data, 0, data.Length, OnWrite, client);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void OnAsyncConnect(PipeStream pipe, out object state)
        {
            var client = new PushAndPullClient();
            client.Stream = pipe;
            client.Id = Interlocked.Increment(ref clientId);

            state = client;
            Thread thread = new Thread(() => PeriodicPush(client));
            thread.Start();
        }
        
        public void OnAsyncDisconnect(PipeStream pipe, object state)
        {
        }

        public void OnAsyncMessage(PipeStream pipe, byte[] data, int bytes, object state)
        {
            PushAndPullClient client = (PushAndPullClient)state;
            Console.WriteLine("Server.PingPong({0}): {1}", client.Id, Encoding.ASCII.GetString(data, 0, bytes));
            pipe.BeginWrite(data, 0, bytes, OnWrite, state);
        }

        private static void OnWrite(IAsyncResult ar)
        {
            PushAndPullClient state = (PushAndPullClient)ar.AsyncState;
            state.Stream.EndWrite(ar);
            Console.WriteLine("Server.WriteDone");
        }

        public void Dispose()
        {
            server.IpcServerStop();
        }
    }

    class Client
    {
        private readonly PipeStream stream;
        private readonly byte[] header = new byte[5000];
        private static int clientId = 0;
        private int id = Interlocked.Increment(ref clientId);

        public Client(string serverName, string name)
        {
            IpcClientPipe clientPipe = new IpcClientPipe(serverName, name);
            stream = clientPipe.Connect(1000);

            stream.BeginRead(header, 0, header.Length, OnRead, this);
        }

        public IAsyncResult BeginWrite(string str)
        {
            byte[] data = Encoding.ASCII.GetBytes(str);
            return stream.BeginWrite(data, 0, data.Length, OnWrite, this);
        }

        private void OnWrite(IAsyncResult ar)
        {
            stream.EndWrite(ar);
        }

        private void OnRead(IAsyncResult ar)
        {
            int n = stream.EndRead(ar);
            if (n > 0)
            {
                Console.WriteLine("Client.Read({0}): {1}", id, Encoding.ASCII.GetString(header, 0, n));

                stream.BeginRead(header, 0, header.Length, OnRead, this);
            }
        }
    }

    class PipeTests
    {
        public static void TestPushAndPull()
        {
            using (PushAndPullServer server = new PushAndPullServer("test"))
            {
                Client client1 = new Client(".", "test");
                Thread.Sleep(100);
                Client client2 = new Client(".", "test");

                Thread.Sleep(1500);
                client1.BeginWrite("from the client1.a");
                client1.BeginWrite("from the client1.b");
                client2.BeginWrite("from the client2.a");
                client2.BeginWrite("from the client2.b");

                Thread.Sleep(1500);
                client1.BeginWrite("from the client1.c");
                Thread.Sleep(1500);
                client2.BeginWrite("from the client2.c");

                Thread.Sleep(10000);
            }
        }
    }
}
