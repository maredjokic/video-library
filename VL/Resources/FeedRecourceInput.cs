using System;
using System.Collections.Generic;


namespace Video_Library_Api.Resources
{
    public class FeedResourceInput
    {
        public string VideoId { get; set; }
        public string URL { get; set; }
        public bool Loop { get; set; }
    }
}