using System.Collections.Generic;
using System.Linq;
using MyServiceLibrary;
using System;
using System.IO;
using System.Reflection;

namespace WcfUserStorageService
{
    public class MasterService : MarshalByRefObject, IMasterService
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

        public int Add(UserDataContract user) => master.Add(new User()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth
        });

        public IEnumerable<int> AddRange(List<UserDataContract> list)
        {
            var userList = list.Select(user => new User()
            {
                FirstName = user.FirstName, LastName = user.LastName, DateOfBirth = user.DateOfBirth
            }).ToList();

            return master.AddRange(userList);
        }

        public bool Remove(UserDataContract user)
        {
            return master.Remove(new User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            });
        }

        public IEnumerable<User> Search(SearchContext search)
        {
            var result = new List<User>();

            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName))
            {
                return master.Search(search.FirstName, search.LastName);
            }

            return result;
        }

        public User SearchById(int id) => master.Search(id);
    }
}
