using System.Collections.Generic;
using System.Linq;
using MyServiceLibrary;
using System;
using System.IO;
using System.Reflection;

namespace WcfUserStorageService
{
    public class MasterService : IMasterService
    {
        private static readonly Master master;

        static MasterService()
        {
            var slaves = new Dictionary<int, string> { { 11000, "127.0.0.1" }, { 11001, "127.0.0.1" }, { 11002, "127.0.0.1" } };
            new SlaveService(); //this line is for scenario when slaves are not created. do we need this?

            //master = new Master(slaves);

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MasterDomain")
            };
            AppDomain domain = AppDomain.CreateDomain("MasterDomain", null, appDomainSetup);
            var assembly = Assembly.Load("MyServiceLibrary");

            master = (Master)domain.CreateInstanceAndUnwrap(assembly.FullName, typeof(Master).FullName, true,
                BindingFlags.Default, null, args: new object[] { slaves }, culture: null,
                activationAttributes: null);
        }

        /// <summary>
        /// Method for adding user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns>id</returns>
        public int Add(UserDataContract user) => master.Add(new User()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth
        });

        /// <summary>
        /// Method for adding range of users
        /// </summary>
        /// <param name="list">list of users</param>
        /// <returns>list of id</returns>
        public IEnumerable<int> AddRange(List<UserDataContract> list)
        {
            var userList = list.Select(user => new User()
            {
                FirstName = user.FirstName, LastName = user.LastName, DateOfBirth = user.DateOfBirth
            }).ToList();

            return master.AddRange(userList);
        }

        /// <summary>
        /// Method for removing users
        /// </summary>
        /// <param name="user">users we want remove</param>
        /// <returns>true on success</returns>
        public bool Remove(UserDataContract user)
        {
            return master.Remove(new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            });
        }

        /// <summary>
        /// Method for removing user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>true on success</returns>
        public bool RemoveById(int id)
        {
            return master.Remove(id);
        }

        /// <summary>
        /// Method for searching user
        /// </summary>
        /// <param name="search">searching context</param>
        /// <returns>list of users</returns>
        public IEnumerable<User> Search(SearchContext search)
        {
            var result = new List<User>();

            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName) && ReferenceEquals(search.DateOfBirth, null))
            {
                return master.Search(search.FirstName, search.LastName).ToList();
            }

            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName) && !ReferenceEquals(search.DateOfBirth, null))
            {
                return master.Search(new User { FirstName = search.FirstName, LastName = search.LastName,
                DateOfBirth = search.DateOfBirth.Value }).ToList();
            }

            return result;
        }

        /// <summary>
        /// Method for searching user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>user</returns>
        public User SearchById(int id) => master.Search(id);
    }
}
