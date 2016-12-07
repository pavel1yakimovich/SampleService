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
            var slaves = new Dictionary<int, string>();

            var config = new MyServiceLibrary.CustomSection.ConfigSettings();

            foreach (var item in config.ServerElements)
            {
                slaves.Add(item.port, item.ip);
            }

            new SlaveService(); //this line is for scenario when slaves are not created. do we need this?
            
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
        public int Add(UserDataContract user)
        {
            if (user.DateOfBirth == null)
            {
                throw new ArgumentNullException();
            }

            return master.Add(Mapper.UserContractToUser(user));
        }

        /// <summary>
        /// Method for adding range of users
        /// </summary>
        /// <param name="list">list of users</param>
        /// <returns>list of id</returns>
        public IEnumerable<int> AddRange(List<UserDataContract> list)
        {
            var range = new List<User>();

            foreach (var user in list)
            {
                if (user.DateOfBirth == null)
                {
                    throw new ArgumentNullException();
                }

                range.Add(Mapper.UserContractToUser(user));
            }

            return master.AddRange(range);
        }

        /// <summary>
        /// Method for removing users
        /// </summary>
        /// <param name="user">users we want remove</param>
        /// <returns>true on success</returns>
        public bool Remove(UserDataContract user)
        {
            if (user.DateOfBirth == null)
            {
                throw new ArgumentNullException();
            }

            return master.Remove(Mapper.UserContractToUser(user));
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
        public IEnumerable<UserDataContract> Search(UserDataContract search)
        {
            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName) && ReferenceEquals(search.DateOfBirth, null))
            {
                return master.Search(search.FirstName, search.LastName).ToList().Select(u => Mapper.UserToUserContract(u));
            }

            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName) && !ReferenceEquals(search.DateOfBirth, null))
            {
                return master.Search(Mapper.UserContractToUser(search)).ToList().Select(u => Mapper.UserToUserContract(u));
            }

            return new List<UserDataContract>();
        }

        /// <summary>
        /// Method for searching user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>user</returns>
        public UserDataContract SearchById(int id) => Mapper.UserToUserContract(master.Search(id));
    }
}
