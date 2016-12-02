using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServiceLibrary
{
    public class Slave : TcpConnector
    {
        private UserStorageService service;
        private Master master;

        public Slave(int port, string ip, UserStorageService service, Master master) : base(port, ip)
        {
            this.service = service;
            this.master = master;
            master.AddSlave(this);
        }

        public IEnumerable<User> Search(Func<User, bool> predicate)
        {
            var result = service.GetUser(predicate);
            
            if (!result.Any())
            {
                this.SendMessage(master.port, master.ipAddr, new Message(Operation.Search, predicate));
            }

            return this.service.GetUser(predicate);
        }
        
        protected override void HandleRequest(Message message)
        {
            switch (message.Operation)
            { 
                case Operation.Search:
                    service.AddRange(message.Parameter as List<User>);
                    break;
                case Operation.Remove:
                    service.Remove(message.Parameter as Predicate<User>);
                    break;
                default:
                    break;
            }
        }
    }
}
