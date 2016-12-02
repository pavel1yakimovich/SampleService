using System;
using System.Collections.Generic;
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

            var slaves = new Dictionary<int, string>();
            slaves.Add(11000, "127.0.0.1");
            slaves.Add(11001, "127.0.0.1");
            slaves.Add(11002, "127.0.0.1");

            var master = new Master(slaves, service);
            var slave1 = new Slave(11000, "127.0.0.1");
            var slave2 = new Slave(11001, "127.0.0.1");
            var slave3 = new Slave(11002, "127.0.0.1");

            var user1 = new User() { FirstName = "Pavel", LastName = "Yakimovich" };
            var user2 = new User() { FirstName = "Pavel", LastName = "Ivanov" };
            master.Add(user1);
            master.Add(user2);

            foreach (var item in slave1.Search(u => u.FirstName == "Pavel"))
            {
                Console.WriteLine(item.LastName);
            }

            master.Remove(u => u.LastName == "Ivanov");

            Console.WriteLine("\nAfter removing slave1: ");

            for (int i = 0; i < 10; i++)
            {
                var list = slave1.Search(u => u.FirstName == "Pavel");
                foreach (var item in list)
                {
                    Console.WriteLine(item.LastName);
                }
            }

            Console.WriteLine("\nAfter removing slave2: ");

            var result = slave1.Search(u => u.FirstName == "Pavel");
            foreach (var item in result)
            {
                Console.WriteLine(item.LastName);
            }


            Console.WriteLine("\nAfter removing slave3: ");

            result = slave1.Search(u => u.FirstName == "Pavel");
            foreach (var item in result)
            {
                Console.WriteLine(item.LastName);
            }

            master.Add(user2);

            Console.WriteLine("\nAfter adding slave3: ");

            result = slave1.Search(u => u.FirstName == "Pavel");

            for (int i = 0; i < 3; i++)
            {
                var list = slave1.Search(u => u.FirstName == "Pavel");
                foreach (var item in list)
                {
                    Console.WriteLine(item.LastName);
                }
            }

            Console.ReadLine();
        }
    }
}
