using System;
using System.Collections.Generic;
using System.Linq;
using MyServiceLibrary.Exceptions;
using MyServiceLibrary.Interfaces;

namespace MyServiceLibrary
{
    [Serializable]
    //// Rename this class. Give the class an appropriate name that will allow all other developers understand it's purpose.
    public class UserStorageService : MarshalByRefObject
    {
        private readonly List<User> storage;
        private readonly Func<int, int> predicateId;
        private int lastId;

        public UserStorageService(List<User> storage = null, Func<int, int> predicate = null)
        {
            if (ReferenceEquals(storage, null))
            {
                storage = new List<User>();
            }

            if (ReferenceEquals(predicate, null))
            {
                predicate = id => ++id;
            }
            
            this.storage = storage;
            this.predicateId = predicate;
            this.lastId = storage.Any() ? storage.LastOrDefault().Id : 0;
        }

        /// <summary>
        /// Method for adding user in storage
        /// </summary>
        /// <param name="user">user we should add</param>
        /// <param name="match">predicate for comparison</param>
        /// <returns>user's id</returns>
        public int Add(User user, Predicate<User> match = null)
        {
            if (ReferenceEquals(user, null))
            {
                throw new ArgumentNullException();
            }

            if (!ReferenceEquals(match, null))
            {
                if (match.Invoke(user))
                {
                    throw new InvalidUserException();
                }
            }

            user.Id = this.SetId();
            this.storage.Add(user);

            return user.Id;
        }

        /// <summary>
        /// Method for adding range of users
        /// </summary>
        /// <param name="users">list of users</param>
        /// <param name="match">predicate for comparison</param>
        /// <returns>users' id</returns>
        public IEnumerable<int> AddRange(IEnumerable<User> users, Predicate<User> match = null)
        {
            if (ReferenceEquals(users, null))
            {
                throw new ArgumentNullException();
            }

            return users.Select(item => this.Add(item, match)).ToList();
        }
        
        /// <summary>
        /// Method for removing users in storage
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>boolean on success</returns>
        public bool Remove(Predicate<User> predicate) => this.storage.RemoveAll(predicate) != 0 ? true : false;
        
        /// <summary>
        /// Method for searching users by predicate
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>list of users</returns>
        public IEnumerable<User> GetUser(Func<User, bool> predicate) => this.storage.Where(predicate).ToList();

        /// <summary>
        /// Method for getting user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>user</returns>
        public User GetUserById(int id) => this.storage.FirstOrDefault(u => u.Id == id);

        /// <summary>
        /// Method for saving list
        /// </summary>
        /// <param name="saver">instance of saver</param>
        public void Save(ISaver<User> saver) => saver.Save(this.storage);

        private int SetId()
        {
            this.lastId = this.predicateId(this.lastId);

            return this.lastId;
        }
    }
}