using System;

namespace Video_Library_Api.Exceptions
{
    public class PropertyDoesntExistException : Exception
    {
        public PropertyDoesntExistException(string message) : base(message)
        {}
    }
}