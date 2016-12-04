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
        [OperationContract]
        User SearchById(int id);

        [OperationContract]
        IEnumerable<User> Search(SearchContext ctx);

        [OperationContract]
        int Add(UserDataContract user);

        [OperationContract]
        IEnumerable<int> AddRange(List<UserDataContract> list);

        [OperationContract]
        bool Remove(UserDataContract user);
    }
    
    [DataContract]
    public class UserDataContract
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime DateOfBirth { get; set; }
    }

    [DataContract]
    public class SearchContext
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public DateTime? DateOfBirth { get; set; }
    }
}
