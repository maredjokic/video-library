using System;

namespace Video_Library_Api.Exceptions
{
    public class DuplicateVideoException : Exception
    {
        public DuplicateVideoException() : base("Video already exists") {}
    }
}