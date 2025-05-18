using System.ComponentModel.DataAnnotations;

namespace Video_Library_Api.Resources
{
    public class VideosTagsDeleteResource
    {
        [Required]
        public string TagName { get; set; }
    }
}