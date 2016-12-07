using System;
using System.Collections.Generic;
using System.IO;
using MyServiceLibrary;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace WcfUserStorageService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class SlaveService : ISlaveService
    {
        private static readonly List<Slave> slaves;

        static SlaveService()
        {
            var dictionary = new Dictionary<int, string>();
            slaves = new List<Slave>();

            dictionary.Add(11000, "127.0.0.1");
            dictionary.Add(11001, "127.0.0.1");
            dictionary.Add(11002, "127.0.0.1");

            foreach (var item in dictionary)
            {
                var appDomainSetup = new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                    PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"SlaveDomain port {item.Key}")
                };
                AppDomain domain = AppDomain.CreateDomain($"SlaveDomain port {item.Key}", null, appDomainSetup);
                var assembly = Assembly.Load("MyServiceLibrary");

                slaves.Add((Slave)domain.CreateInstanceAndUnwrap(assembly.FullName, typeof(Slave).FullName, true,
                    BindingFlags.Default, null, args: new object[] { item.Key, item.Value }, culture: null,
                    activationAttributes: null));
                //slaves.Add(new Slave(item.Key, item.Value));
            }
        }

        /// <summary>
        /// Method for searchiong user
        /// </summary>
        /// <param name="search">searching context</param>
        /// <param name="slaveNumber">number of slave</param>
        /// <returns></returns>
        public IEnumerable<User> Search(SearchContext search, int slaveNumber)
        {
            var slave = slaves[slaveNumber];
            var result = new List<User>();

            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName) && ReferenceEquals(search.DateOfBirth, null))
            {
                return slave.Search(search.FirstName, search.LastName).ToList();
            }

            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName) && !ReferenceEquals(search.DateOfBirth, null))
            {
                return slave.Search(new User
                {
                    FirstName = search.FirstName,
                    LastName = search.LastName,
                    DateOfBirth = search.DateOfBirth.Value
                }).ToList();
            }

            return result;
        }

        /// <summary>
        /// Method for searching user by Id
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="slaveNumber">number of slave</param>
        /// <returns></returns>
        public User SearchById (int id, int slaveNumber)
        {
            return slaves[slaveNumber].Search(id);
        }
    }
}