using System;
using System.Collections.Generic;
using System.ServiceModel;
using MyServiceLibrary;

namespace WcfUserStorageService
{
    [ServiceContract]
    public interface ISlaveService
    {
        /// <summary>
        /// Method for searching user
        /// </summary>
        /// <param name="search">Search context</param>
        /// <param name="slaveNumber">number of slave</param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<UserDataContract> Search(UserDataContract search, int slaveNumber);

        /// <summary>
        /// Method for searching user by id
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="slaveNumber">number of slave</param>
        /// <returns></returns>
        [OperationContract]
        UserDataContract SearchById(int id, int slaveNumber);
    }
}
