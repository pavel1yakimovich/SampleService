using System;

namespace MyServiceLibrary.Exceptions
{
    public class InvalidUserException : Exception
    {
        public InvalidUserException() : base()
        {
        }

        public InvalidUserException(string message) : base(message)
        {
        }
    }
}
