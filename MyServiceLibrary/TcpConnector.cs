using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    [Serializable]
    public class TcpConnector
    {
        public int port;
        public string ipAddr;

        public TcpConnector(int listenPort, string ipAddr)
        {
            this.port = listenPort;
            this.ipAddr = ipAddr;

            Task.Run(() => CreateServer());
        }

        private Task CreateServer()
        {
            TcpListener server = null;
            Message message = null;

            IPAddress localAddr = IPAddress.Parse(ipAddr);
            server = new TcpListener(localAddr, this.port);

            // Start listening for client requests. 
            server.Start();

            var formatter = new BinaryFormatter();

            // Enter the listening loop. 
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                using (NetworkStream stream = client.GetStream())
                {
                    message = (Message)formatter.Deserialize(stream);
                    HandleRequest(stream, message);
                }
                client.Close();
            }
        }

        protected virtual void HandleRequest(NetworkStream stream, Message message)
        {
        }

        protected void SendMessage(int port, string ip, Message message)
        {
            if (ReferenceEquals(message, null))
            {
                return;
            }

            var formatter = new BinaryFormatter();

            using (TcpClient client = new TcpClient(ip, port))
            {
                using (NetworkStream stream = client.GetStream())
                {
                    formatter.Serialize(stream, message);
                }
            }
        }
    }
}
