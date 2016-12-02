using System;
using System.Collections.Generic;
using System.Linq;

namespace MyServiceLibrary
{
    [Serializable]
    // Rename this class. Give the class an appropriate name that will allow all other developers understand it's purpose.
    public class UserStorageService
    {
        private List<User> storage;
        private Func<int, int> predicateId;
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
        /// <returns>this user</returns>
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

        public IEnumerable<int> AddRange(IEnumerable<User> users, Predicate<User> match = null)
        {
            if (ReferenceEquals(users, null))
            {
                throw new ArgumentNullException();
            }

            var result = new List<int>();

            foreach(var item in users)
            {
                result.Add(this.Add(item, match));
            }

            return result;
        }

        /// <summary>
        /// Method for removing user in storage
        /// </summary>
        /// <param name="user">user we should remove</param>
        /// <returns>boolean on success</returns>
        public void Remove(Predicate<User> predicate) => this.storage.RemoveAll(predicate);

        /// <summary>
        /// Method for searching user by predicate
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>IEnumerable of users</returns>
        public IEnumerable<User> GetUser(Func<User, bool> predicate) => this.storage.Where(predicate);

        /// <summary>
        /// Method for getting user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>user</returns>
        public User GetUserById(int id) => this.storage.FirstOrDefault(u => u.Id == id);

        /// <summary>
        /// Method for saving list
        /// </summary>
        public void Save(ISaver<User> saver) => saver.Save(this.storage);

        private int SetId()
        {
            this.lastId = this.predicateId(this.lastId);

            return this.lastId;
        }
    }
}