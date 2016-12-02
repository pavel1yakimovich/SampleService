using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyServiceLibrary.Exceptions;

namespace MyServiceLibrary.Tests
{
    [TestClass]
    public class MyServiceTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullUser_ExceptionThrown()
        {
            var service = new UserStorageService();

            service.Add(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidUserException))]
        public void Add_InvalidUsersLastName_ExceptionThrown()
        {
            var service = new UserStorageService();

            service.Add(new User() { LastName = "q" }, u => u.FirstName == null);
        }

        [TestMethod]
        public void Add_User()
        {
            var service = new UserStorageService();
            var user1 = new User() { FirstName = "John", LastName = "Brown" };
            service.Add(user1);
            service.Add(new User() { FirstName = "Chris", LastName = "Brown" });
            service.Add(new User() { FirstName = "Chris", LastName = "Hamsworth" });

            Assert.AreEqual(service.GetUser(u => u.FirstName == user1.FirstName).First().Id, 1);
        }

        [TestMethod]
        public void Remove_User()
        {
            var service = new UserStorageService();
            var user = new User() { FirstName = "John", LastName = "Smiths", DateOfBirth = new DateTime(1995, 09, 15, 14, 45, 00) };
            int id = service.Add(user);

            service.Remove(u => u.Id == id);

            Assert.AreEqual(null, service.GetUserById(id));
        }

        [TestMethod]
        public void GetNonExistUser()
        {
            var service = new UserStorageService();
            var nullUser = service.GetUser(u => u.FirstName == "Pavel" && u.LastName == "Yakimovich"
             && u.DateOfBirth == DateTime.Now);

            Assert.AreEqual(0, nullUser.Count());
        }

        [TestMethod]
        public void GetUser()
        {
            var service = new UserStorageService();
            var user = new User() { FirstName = "John", LastName = "Smiths", DateOfBirth = new DateTime(1995, 09, 15, 14, 45, 00) };

            service.Add(user);
            User userActual = service.GetUser(u => u.FirstName == user.FirstName && u.LastName == user.LastName
             && u.DateOfBirth == user.DateOfBirth).FirstOrDefault();

            Assert.AreEqual(user, userActual);
        }

        [TestMethod]
        public void GetTwoUsers()
        {
            var service = new UserStorageService();
            var user1 = new User() { FirstName = "John", LastName = "Smiths", DateOfBirth = new DateTime(1995, 09, 15, 14, 45, 00) };
            var user2 = new User() { FirstName = "Johnathan", LastName = "Smiths", DateOfBirth = new DateTime(1995, 09, 15, 14, 45, 00) };
            var user3 = new User() { FirstName = "Chris", LastName = "Smiths", DateOfBirth = new DateTime(1995, 09, 15, 14, 45, 00) };

            service.Add(user1);
            service.Add(user2);
            service.Add(user3);

            var users = service.GetUser(u => u.FirstName.Contains(user1.FirstName));

            Assert.IsTrue(users.Contains(user1) && users.Contains(user2));
        }

        [TestMethod]
        public void GetUserById()
        {
            var service = new UserStorageService();
            service.Add(new User() { FirstName = "Johnathan", LastName = "Smiths", DateOfBirth = new DateTime(1995, 09, 15, 14, 45, 00) });
            var user = new User() { FirstName = "John", LastName = "Smiths", DateOfBirth = new DateTime(1995, 09, 15, 14, 45, 00) };
            service.Add(user);
            service.Add(new User() { FirstName = "Chris", LastName = "Smiths", DateOfBirth = new DateTime(1995, 09, 15, 14, 45, 00) });

            int idActual = service.GetUserById(2).Id;

            Assert.AreEqual(user.Id, idActual);
        }
    }
}