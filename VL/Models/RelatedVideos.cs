using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Video_Library_Api.Models
{
    public class RelatedVideos
    {
        public string Video1Id { get; set; }
        public string Video2Id { get; set; }
        public int Offset1 { get; set; }
        public int Offset2 { get; set; }
        public int Length { get; set; }
        public float Score { get; set; }

        public Video Video1 { get; set; }
        public Video Video2 { get; set; }
    }
}