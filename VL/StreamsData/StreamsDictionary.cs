using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Video_Library_Api.Models;
using Video_Library_Api.Resources;
using System.Threading.Tasks;

namespace Video_Library_Api.StreamsData 
{
    public class StreamsDictionary
    {
        public Dictionary<int, FeedProcessResource> mdspprocProcesses;

        public StreamsDictionary()
        {
            mdspprocProcesses = new Dictionary<int, FeedProcessResource>();
        }
    }
}