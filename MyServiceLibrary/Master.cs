﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    public class Master : MarshalByRefObject
    {
        private readonly UserStorageService service;
        private readonly Dictionary<int, string> slaves;

        public Master(Dictionary<int, string> slaves) : this(slaves, null)
        {
        }

        public Master(Dictionary<int, string> slaves, UserStorageService service)
        {
            if (ReferenceEquals(service, null))
            {
                service = new UserStorageService();
            }

            this.service = service;
            this.slaves = slaves;
        }

        /// <summary>
        /// Method for adding user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns>id</returns>
        public int Add(User user)
        {
            int result = this.service.Add(user);
            foreach (var slave in this.slaves)
            {
                this.SendMessage(slave.Key, slave.Value, new Message(Operation.Add, user));
            }

            return result;
        }

        /// <summary>
        /// Method for adding a range of users
        /// </summary>
        /// <param name="list">list of users</param>
        /// <returns>list of id</returns>
        public IEnumerable<int> AddRange(IEnumerable<User> list)
        {
            IEnumerable<int> result = this.service.AddRange(list);

            foreach (var slave in this.slaves)
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
        public User Search(int id) => this.service.GetUserById(id);

        /// <summary>
        /// Method for searching user by name
        /// </summary>
        /// <param name="fname">Firstname</param>
        /// <param name="lname">Lastname</param>
        /// <returns>list of users</returns>
        public IEnumerable<User> Search(string fname, string lname) => this.service.GetUser(u => u.FirstName == fname && u.LastName == lname);

        /// <summary>
        /// Method for searching user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns>list of users</returns>
        public IEnumerable<User> Search(User user) => this.service.GetUser(
            u => u.FirstName == user.FirstName && user.LastName == user.LastName 
            && u.DateOfBirth == user.DateOfBirth);

        /// <summary>
        /// Method for removing user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns>true on seccess</returns>
        public bool Remove(User user)
        {
            bool result = this.service.Remove(u => u.Equals(user));

            foreach (var slave in this.slaves)
            {
                this.SendMessage(slave.Key, slave.Value, new Message(Operation.Remove, user));
            }

            return result;
        }

        /// <summary>
        /// Method for removing user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>true on seccess</returns>
        public bool Remove(int id)
        {
            var user = this.service.GetUserById(id);
            bool result = this.service.Remove(u => u.Id == id);

            foreach (var slave in this.slaves)
            {
                this.SendMessage(slave.Key, slave.Value, new Message(Operation.Remove, user));
            }

            return result;
        }

        private void SendMessage(int port, string ip, Message message)
        {
            if (ReferenceEquals(message, null))
            {
                return;
            }

            var formatter = new BinaryFormatter();

            try
            {

                using (TcpClient client = new TcpClient(ip, port))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        formatter.Serialize(stream, message);
                    }
                }
            }
            catch (SocketException)
            {
            }
        }
    }
}
