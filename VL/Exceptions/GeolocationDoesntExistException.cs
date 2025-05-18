using System;

namespace Video_Library_Api.Exceptions
{
    public class GeolocationDoesntExistException : Exception
    {
        public GeolocationDoesntExistException(string message) : base(message)
        {}
    }
}