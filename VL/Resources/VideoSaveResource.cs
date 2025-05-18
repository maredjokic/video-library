using System.ComponentModel.DataAnnotations;

namespace Video_Library_Api.Resources
{
    public class VideoSaveResource
    {
        [Required]
        public string FileName { get; set; }
    }
}