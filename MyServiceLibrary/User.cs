using System;
using System.Runtime.Serialization;

namespace MyServiceLibrary
{
    [Serializable]
    public class User
    {
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public DateTime DateOfBirth { get; set; }

        public int Id { get; set; }
        
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            var user = obj as User;

            if (ReferenceEquals(user, null))
            {
                return false;
            }

            return this.FirstName == user.FirstName && this.LastName == user.LastName
                && this.DateOfBirth == user.DateOfBirth;
        }
    }


}
