using System;
using System.Collections.Generic;
using System.ServiceModel;
using MyServiceLibrary;
using System.Runtime.Serialization;

namespace WcfUserStorageService
{
    [ServiceContract]
    public interface IMasterService
    {
        /// <summary>
        /// Method for searching user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>user</returns>
        [OperationContract]
        UserDataContract SearchById(int id);

        /// <summary>
        /// Method for searching user
        /// </summary>
        /// <param name="ctx">searching context</param>
        /// <returns>list of users</returns>
        [OperationContract]
        IEnumerable<UserDataContract> Search(UserDataContract ctx);

        /// <summary>
        /// Method for adding user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns>id</returns>
        [OperationContract]
        int Add(UserDataContract user);

        /// <summary>
        /// Method for adding range of users
        /// </summary>
        /// <param name="list">list of users</param>
        /// <returns>list of id</returns>
        [OperationContract]
        IEnumerable<int> AddRange(List<UserDataContract> list);

        /// <summary>
        /// Method for removing users
        /// </summary>
        /// <param name="user">users we want remove</param>
        /// <returns>true on success</returns>
        [OperationContract]
        bool Remove(UserDataContract user);

        /// <summary>
        /// Method for removing user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>true on success</returns>
        [OperationContract]
        bool RemoveById(int id);
    }
    
    [DataContract]
    public class UserDataContract
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime? DateOfBirth { get; set; }
    }
}
