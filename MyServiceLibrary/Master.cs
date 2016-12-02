using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    public class Master
    {
        private UserStorageService service;
        private Dictionary<int, string> slaves;

        public Master(int port, string ip, UserStorageService service)
        {
            this.service = service;
            this.slaves = new Dictionary<int, string>();
            this.slaves.Add(11000, "127.0.0.1");
        }

        /// <summary>
        /// Method for adding user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns>id</returns>
        public int Add(User user)
        {
            int result = service.Add(user);

            foreach(var slave in slaves)
            {
                this.SendMessage(slave.Key, slave.Value, new Message(Operation.Add, user));
            }

            return result;
        }

        public IEnumerable<int> AddRange(IEnumerable<User> list)
        {
            IEnumerable<int> result = service.AddRange(list);

            foreach (var slave in slaves)
            {
                this.SendMessage(slave.Key, slave.Value, new Message(Operation.AddRange, list));
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
                this.SendMessage(slave.Key, slave.Value, new Message(Operation.Remove, predicate));
            }
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
