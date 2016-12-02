using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    public class Master : TcpConnector
    {
        private UserStorageService service;
        private List<Slave> slaves;

        public Master(int port, string ip, UserStorageService service) : base(port, ip)
        {
            this.service = service;
            this.slaves = new List<Slave>();
        }

        public int Add(User user) => service.Add(user);

        public User SearchById(int id) => service.GetUserById(id);

        public IEnumerable<User> Search(Func<User, bool> predicate) => service.GetUser(predicate);

        public void Remove(Predicate<User> predicate)
        {
            service.Remove(predicate);
            foreach(var slave in slaves)
            {
                this.SendMessage(slave.port, slave.ipAddr, new Message(Operation.Remove, predicate));
            }
        }

        public void AddSlave(Slave slave) => slaves.Add(slave);

        protected override void HandleRequest(Message message)
        {

        }
    }
}
