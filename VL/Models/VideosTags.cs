using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Video_Library_Api.Models
{
    public class VideosTags
    {
        public string VideoId { get; set; }

        public int TagId { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        
        public Video Video { get; set; }
        public Tag Tag { get; set; }

    }
}