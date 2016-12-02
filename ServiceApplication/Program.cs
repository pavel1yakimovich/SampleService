using System;
using MyServiceLibrary;
using System.Collections.Generic;

namespace ServiceApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var service = new UserStorageService();

            var slaveService = new UserStorageService();
            // 1. Add a new user to the storage.
            // 2. Remove an user from the storage.
            // 3. Search for an user by the first name.
            // 4. Search for an user by the last name.
            var master = new Master(11001, "127.0.0.1", service);
            var slave = new Slave(11000, "127.0.0.1", slaveService, master);
            
            var user1 = new User() { FirstName = "Pavel", LastName = "Yakimovich" };
            var user2 = new User() { FirstName = "Pavel", LastName = "Ivanov" };
            master.Add(user1);
            master.Add(user2);
            
            Console.ReadLine();

            foreach (var item in slave.Search(u => u.FirstName == "Pavel"))
            {
                Console.WriteLine(item.LastName);
            }

            master.Remove(u => u.LastName == "Ivanov");

            Console.WriteLine("\nAfter removing: ");

            Console.ReadLine();

            foreach (var item in slave.Search(u => u.FirstName == "Pavel"))
            {
                Console.WriteLine(item.LastName);
            }

            Console.ReadLine();
        }
    }
}
