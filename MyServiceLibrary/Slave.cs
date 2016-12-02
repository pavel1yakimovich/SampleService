using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyServiceLibrary
{
    public class Slave : TcpConnector
    {
        private UserStorageService service;
        private Master master;

        public Slave(int port, string ip, UserStorageService service, Master master) : base(port, ip)
        {
            this.service = service;
            this.master = master;
            master.AddSlave(this);
        }

        /// <summary>
        /// Method for searching users
        /// </summary>
        /// <param name="predicate">Predicate</param>
        /// <returns>List of users</returns>
        public IEnumerable<User> Search(Func<User, bool> predicate) => this.service.GetUser(predicate);
        
        protected override void HandleRequest(NetworkStream stream, Message message)
        {
            var formatter = new BinaryFormatter();

            switch (message.Operation)
            { 
                case Operation.Add:
                    service.Add(message.Parameter as User);
                    break;
                case Operation.AddRange:
                    service.AddRange(message.Parameter as List<User>);
                    break;
                case Operation.Remove:
                    service.Remove(message.Parameter as Predicate<User>);
                    break;
                default:
                    break;
            }
        }
    }
}
