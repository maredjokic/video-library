using System;

namespace Video_Library_Api.Exceptions
{
    public class TagExistsException : Exception
    {
        public TagExistsException(string message) : base(message) {}
    }
}