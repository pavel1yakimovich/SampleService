using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MyServiceLibrary;

namespace WcfUserStorageService
{
    public class MasterService : IMasterService
    {
        private Master master;

        public MasterService()
        {
            var slaves = new Dictionary<int, string>();
            slaves.Add(11000, "127.0.0.1");
            slaves.Add(11001, "127.0.0.1");
            slaves.Add(11002, "127.0.0.1");

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyDomain")
            };
            AppDomain domain = AppDomain.CreateDomain("MyDomain", null, appDomainSetup);
            var assembly = Assembly.Load("MyServiceLibrary");

            this.master = (Master)domain.CreateInstanceAndUnwrap("MyServiceLibrary", typeof(Master).FullName, true, BindingFlags.Default, null, args: new object[] { slaves }, culture: null, activationAttributes: null);
        }

        public int Add(User user)
        {
            var slaves = new Dictionary<int, string>();
            slaves.Add(11000, "127.0.0.1");
            slaves.Add(11001, "127.0.0.1");
            slaves.Add(11002, "127.0.0.1");

            master = new Master(slaves);
            return this.master.Add(user);
        }

        public IEnumerable<int> AddRange(IEnumerable<User> list) => this.master.AddRange(list);

        public void Remove(PredicateContract predicate) => this.master.Remove(predicate.ContractPredicate as Predicate<User>);

        public IEnumerable<User> Search(PredicateContract predicate) => this.master.Search(predicate.ContractFunc as Func<User, bool>);

        public User SearchById(int id) => this.master.SearchById(id);
    }
}
