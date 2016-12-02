using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    public class Slave 
    {
        private UserStorageService service;
        private Master master;
        internal int port;
        internal string ipAddr;
        private object locker = new object();

        public Slave(int port, string ip, UserStorageService service, Master master)
        {
            this.service = service;
            this.master = master;
            this.ipAddr = ip;
            this.port = port;

            Task.Run(() => this.CreateServer());
        }

        /// <summary>
        /// Method for searching users
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>List of users</returns>
        public IEnumerable<User> Search(Func<User, bool> predicate)
        {
            IEnumerable<User> result;

            lock (locker)
            {
                result = this.service.GetUser(predicate);
            }

            return result;
        }

        private Task CreateServer()
        {
            IPAddress localAddr = IPAddress.Parse(ipAddr);
            TcpListener server = new TcpListener(localAddr, this.port);

            // Start listening for client requests. 
            server.Start();

            var formatter = new BinaryFormatter();

            // Enter the listening loop. 
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();

                using (NetworkStream stream = client.GetStream())
                {
                    Message message = (Message)formatter.Deserialize(stream);
                    HandleRequest(stream, message);
                }
                client.Close();
            }
        }

        private void HandleRequest(NetworkStream stream, Message message)
        {
            var formatter = new BinaryFormatter();

                switch (message.Operation)
                {
                    case Operation.Add:
                        lock (locker)
                        {
                            service.Add(message.Parameter as User);
                        }
                        break;
                    case Operation.AddRange:
                        lock (locker)
                        {
                            service.AddRange(message.Parameter as List<User>);
                        }
                        break;
                    case Operation.Remove:
                        lock (locker)
                        {
                            service.Remove(message.Parameter as Predicate<User>);
                        }
                        break;
                    default:
                        break;
               }
            
        }
    }
}
