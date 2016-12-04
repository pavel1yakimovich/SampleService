using System;
using System.Collections.Generic;
using System.ServiceModel;
using MyServiceLibrary;

namespace WcfUserStorageService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISlaveService
    {
        [OperationContract]
        IEnumerable<User> Search(SearchContext search, int slaveNumber);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WcfUserStorageService.ContractType".
}
