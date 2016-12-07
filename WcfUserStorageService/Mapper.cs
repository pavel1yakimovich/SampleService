using MyServiceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfUserStorageService
{
    public static class Mapper
    {
        public static UserDataContract UserToUserContract(User user)
        {
            return new UserDataContract
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            };
        }

        public static User UserContractToUser(UserDataContract user)
        {
            return new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth.Value
            };
        }
    }
}
