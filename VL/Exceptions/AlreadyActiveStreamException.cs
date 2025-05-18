using System;

namespace Video_Library_Api.Exceptions
{
    public class AlreadyActiveStreamException : Exception
    {
        public AlreadyActiveStreamException() : base("The stream is already active!") {}
    }
}