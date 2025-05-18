using System;
using System.Collections.Generic;


namespace Video_Library_Api.Resources
{
    public class DirectoryInfResource
    {
        public string DirectoryHash { get; set; }
        public string Path { get; set; }
        public string Status { get; set; }
        public int FinishedEntries { get; set; }
        public int TotalEntries {get; set;}
    }
}