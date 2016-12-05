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
    public class SlaveService : MarshalByRefObject, ISlaveService
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

        public IEnumerable<User> Search(SearchContext search, int slaveNumber)
        {
            var slave = slaves[slaveNumber];
            var result = new List<User>();

            if (!string.IsNullOrEmpty(search.FirstName) && !string.IsNullOrEmpty(search.LastName))
            {
                return slave.Search(search.FirstName, search.LastName);
            }

            return result;
        }

        public User SearchById (int id, int slaveNumber)
        {
            return slaves[slaveNumber].Search(id);
        }
    }
}


//public delegate IList<User> SearchDelegate(params Func<User, bool>[] func);
//class Program
//{
//    private static SearchDelegate search;
//    static void Main(string[] args)
//    {
//        #region newDomain 
//        var appDomainSetup = new AppDomainSetup
//        {
//            ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
//            PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FirstSlave")
//        };

//        AppDomain domain = AppDomain.CreateDomain("FirstSlave", null, appDomainSetup);
//        #endregion

//        var slave = (Slave)domain.CreateInstanceAndUnwrap("MasterSlaveReplication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", typeof(Slave).FullName);
//        var slaveTask = Task.Run(() => slave.ListenMaster());
//        var master = new Master();
//        search += master.SearchUsers;
//        search += slave.SearchUsers;



//        master.Add(new User() { FirstName = "Pasha", LastName = "Yakimovich", DateOfBirth = DateTime.Now });
//        master.Add(new User() { FirstName = "Pasha", LastName = "Yakimovich-wqeweg", DateOfBirth = DateTime.Now });

//        master.Update(new User() { Id = 1, FirstName = "Pasha", LastName = "Yakimovicjirgp", DateOfBirth = DateTime.Now });
//        ImitateRequest(5);



//        slaveTask.Wait();