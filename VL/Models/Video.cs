using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Video_Library_Api.Models
{
    public class Video
    {
        [MaxLength(16)]
        public string Id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(60)]
        public string FormatLongName { get; set; }

        [MaxLength(100)]
        public string CodecName { get; set; }

        public double? Duration { get; set; }
 
        public long? Size { get; set; }

        public long? BitRate { get; set; }
        
        public int? Width { get; set; }
        
        public int? Height { get; set; }

        [Required]
        [MaxLength(4096)]
        public string StreamsJSON { get; set; }

        public DateTime? CreationTime { get; set; }
        [Required]
        public DateTime? UploadTime { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int ProcessesLeft { get; set; }
        public string StoragePath { get; set; }

        public VideoGeolocation VideoGeolocation { get; set; }
        public ICollection<VideosTags> VideosTags { get; set; }
        public ICollection<RelatedVideos> RelatedToVideos { get; set; }
        public ICollection<RelatedVideos> RelatedFromVideos { get; set; }
    }
}