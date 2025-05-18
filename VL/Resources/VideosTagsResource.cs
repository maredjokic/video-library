using System.ComponentModel.DataAnnotations;

namespace Video_Library_Api.Resources
{
    public class VideosTagsResource
    {
        [Required]
        public string TagName { get; set; }
        [Required]
        public int From { get; set; }
        [Required]
        public int To { get; set; } = int.MaxValue;
    }
}