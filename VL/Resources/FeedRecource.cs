using System;
using System.Collections.Generic;


namespace Video_Library_Api.Resources
{
    public class FeedResource
    {
        public string VideoId { get; set; }
        public int FeedId { get; set; }
        public string URL { get; set; }
        public bool Loop { get; set; }
        public bool Active { get; set; }
    }
}