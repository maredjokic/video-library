using System;

namespace Video_Library_Api.Exceptions
{
    public class NotUserAddedTagException : Exception
    {
        public NotUserAddedTagException(string message) :
            base(message)
        {}
    }
}