using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    public class Master : TcpConnector
    {
        private UserStorageService service;
        private List<Slave> slaves;

        public Master(int port, string ip, UserStorageService service) : base(port, ip)
        {
            this.service = service;
            this.slaves = new List<Slave>();
        }

        /// <summary>
        /// Method for adding user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns>id</returns>
        public int Add(User user)
        {
            int result = service.Add(user);

            foreach(Slave slave in slaves)
            {
                this.SendMessage(slave.port, slave.ipAddr, new Message(Operation.Add, user));
            }

            return result;
        }

        public IEnumerable<int> AddRange(IEnumerable<User> list)
        {
            IEnumerable<int> result = service.AddRange(list);

            foreach (Slave slave in slaves)
            {
                this.SendMessage(slave.port, slave.ipAddr, new Message(Operation.AddRange, list));
            }

            return result;
        }

        /// <summary>
        /// Method for searching users by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>User</returns>
        public User SearchById(int id) => service.GetUserById(id);

        /// <summary>
        /// Method for searching users by predicate
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>List of users</returns>
        public IEnumerable<User> Search(Func<User, bool> predicate) => service.GetUser(predicate);

        /// <summary>
        /// Method for removing users by predicate
        /// </summary>
        /// <param name="predicate">predicate</param>
        public void Remove(Predicate<User> predicate)
        {
            service.Remove(predicate);
            foreach(var slave in slaves)
            {
                this.SendMessage(slave.port, slave.ipAddr, new Message(Operation.Remove, predicate));
            }
        }

        internal void AddSlave(Slave slave) => slaves.Add(slave);
    }
}
