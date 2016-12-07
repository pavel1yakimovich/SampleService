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

            var config = new MyServiceLibrary.CustomSection.ConfigSettings();

            foreach (var item in config.ServerElements)
            {
                var appDomainSetup = new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                    PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"SlaveDomain port {item.port} ip {item.ip}")
                };
                AppDomain domain = AppDomain.CreateDomain($"SlaveDomain port {item.port} ip {item.ip}", null, appDomainSetup);
                var assembly = Assembly.Load("MyServiceLibrary");

                slaves.Add((Slave)domain.CreateInstanceAndUnwrap(assembly.FullName, typeof(Slave).FullName, true,
                    BindingFlags.Default, null, args: new object[] { item.port, item.ip }, culture: null,
                    activationAttributes: null));
            }
        }

        /// <summary>
        /// Method for searchiong user
        /// </summary>
        /// <param name="search">searching context</param>
        /// <param name="slaveNumber">number of slave</param>
        /// <returns></returns>
        public IEnumerable<UserDataContract> Search(UserDataContract search, int slaveNumber)
        {
            var slave = slaves[slaveNumber];

            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName) && ReferenceEquals(search.DateOfBirth, null))
            {
                return slave.Search(search.FirstName, search.LastName).ToList().Select(u => Mapper.UserToUserContract(u));
            }

            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName) && !ReferenceEquals(search.DateOfBirth, null))
            {
                return slave.Search(Mapper.UserContractToUser(search)).ToList().Select(u => Mapper.UserToUserContract(u));
            }

            return new List<UserDataContract>();
        }

        /// <summary>
        /// Method for searching user by Id
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="slaveNumber">number of slave</param>
        /// <returns></returns>
        public UserDataContract SearchById (int id, int slaveNumber)
        {
            return Mapper.UserToUserContract(slaves[slaveNumber].Search(id));
        }
    }
}