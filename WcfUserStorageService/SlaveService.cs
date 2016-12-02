using System;
using System.Collections.Generic;
using MyServiceLibrary;
using System.IO;
using System.Reflection;

namespace WcfUserStorageService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class SlaveService : ISlaveService
    {
        private Slave slave;

        public SlaveService()
        {
            int port = 11000;
            string ip = "127.0.0.1";

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyDomain")
            };
            AppDomain domain = AppDomain.CreateDomain("MyDomain", null, appDomainSetup);
            var assembly = Assembly.Load("MyServiceLibrary");

            this.slave = (Slave)domain.CreateInstanceAndUnwrap("MyServiceLibrary", typeof(Slave).FullName, true, BindingFlags.Default, null, args: new object[] { port, ip }, culture: null, activationAttributes: null);
        }

        public IEnumerable<User> Search(Func<User, bool> predicate) => this.slave.Search(predicate);
    }
}
