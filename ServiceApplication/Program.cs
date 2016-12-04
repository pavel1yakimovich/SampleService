using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MyServiceLibrary;

namespace ServiceApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var service = new UserStorageService();
            // 1. Add a new user to the storage.
            // 2. Remove an user from the storage.
            // 3. Search for an user by the first name.
            // 4. Search for an user by the last name.

            var dictionary = new Dictionary<int, string>();
            dictionary.Add(11000, "127.0.0.1");
            //dictionary.Add(11001, "127.0.0.1");
            //dictionary.Add(11002, "127.0.0.1");

            var slaves = new List<Slave>();

            foreach (var item in dictionary)
            {
                var appDomainSetup = new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                    PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SlaveDomain")
                };
                AppDomain domain = AppDomain.CreateDomain("SlaveDomain", null, appDomainSetup);
                var assembly = Assembly.Load("MyServiceLibrary");

                slaves.Add((Slave)domain.CreateInstanceAndUnwrap(assembly.FullName, typeof(Slave).FullName, true,
                    BindingFlags.Default, null, args: new object[] { item.Key, item.Value }, culture: null,
                    activationAttributes: null));
                //slaves.Add(new Slave(item.Key, item.Value));
            }

            var appDomainSetup1 = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Master")
            };
            AppDomain domain1 = AppDomain.CreateDomain("Master", null, appDomainSetup1);
            var assembly1 = Assembly.Load("MyServiceLibrary");

            var master = (Master)domain1.CreateInstanceAndUnwrap(assembly1.FullName, typeof(Master).FullName, true,
                BindingFlags.Default, null, args: new object[] { dictionary }, culture: null,
                activationAttributes: null);

            var user1 = new User() { FirstName = "Pavel", LastName = "Yakimovich" };
            var user2 = new User() { FirstName = "Pavel", LastName = "Ivanov" };
            master.Add(user1);
            master.Add(user2);

            Console.ReadLine();

            slaves[0].Search(u => u.Id == 1);

            Console.ReadLine();
        }
    }
}
