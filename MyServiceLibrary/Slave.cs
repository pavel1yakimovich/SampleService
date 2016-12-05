using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    [Serializable]
    public class Slave : MarshalByRefObject
    {
        private static int count;
        private UserStorageService service;
        private object locker = new object();

        internal int Port { get; set; }

        internal string IPAddr { get; set; }

        public Slave(int port, string ip) : this(port, ip, null)
        {
        }

        public Slave(int port, string ip, UserStorageService service)
        {
            if (ReferenceEquals(service, null))
            {
                service = new UserStorageService();
            }

            this.service = service;
            this.IPAddr = ip;
            this.Port = port;
            count++;

            Task.Run(() => this.CreateServer());
        }

        /// <summary>
        /// Method for searching users
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>List of users</returns>
        public IEnumerable<User> Search(string fname, string lname)
        {
            IEnumerable<User> result;

            lock (this.locker)
            {
                result = this.service.GetUserByName(fname, lname);
            }

            return result;
        }

        public User Search(int id)
        {
            User result;

            lock (this.locker)
            {
                result = this.service.GetUserById(id);
            }

            return result;
        }

        private Task CreateServer()
        {
            IPAddress localAddr = IPAddress.Parse(this.IPAddr);
            TcpListener server = new TcpListener(localAddr, this.Port);

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
                    this.HandleRequest(stream, message);
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
                    lock (this.locker)
                    {
                        this.service.Add(message.Parameter as User);
                    }

                    break;
                case Operation.AddRange:
                    lock (this.locker)
                    {
                        this.service.AddRange(message.Parameter as List<User>);
                    }

                    break;
                case Operation.Remove:
                    lock (this.locker)
                    {
                        var user = message.Parameter as User;
                        this.service.Remove(u => u.Equals(user));
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
