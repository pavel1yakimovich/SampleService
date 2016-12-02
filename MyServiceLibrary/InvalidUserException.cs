using System;

namespace MyServiceLibrary
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
