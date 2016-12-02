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
        IEnumerable<User> Search(PredicateContract predicate);

        [OperationContract]
        int Add(User user);

        [OperationContract]
        IEnumerable<int> AddRange(IEnumerable<User> list);

        [OperationContract]
        void Remove(PredicateContract predicate);
    }
    
    [DataContract]
    [KnownType(typeof(object))]
    public class PredicateContract
    {
        [DataMember]
        public object ContractFunc { get; set; }

        [DataMember]
        public object ContractPredicate { get; set; }
    }
}
