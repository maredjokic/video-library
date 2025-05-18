using System;
using System.Collections.Generic;

namespace Video_Library_Api.Resources
{
    public class VideoResource
    {
        public string Id { get; set; }
        public string FileName { get; set; }

        public string FormatLongName { get; set; }
        public string CodecName { get; set; }
        public double Duration { get; set; }
        public long Size { get; set; }
        public long BitRate { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string StreamsJSON { get; set; }
        public DateTime? CreationTime { get; set; }
        public DateTime UploadTime { get; set; }
        public string Video { get; set; }
        public string TranscodedVideo { get; set; }
        public string Thumbnail { get; set; }
        public string Preview { get; set; }
        public string FFProbe { get; set; }
        public string KLV2JSON { get; set; }
        public int ProcessesLeft { get; set; }
        public string StoragePath { get; set;}
    }
}