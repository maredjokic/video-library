using System.ComponentModel.DataAnnotations;

namespace Video_Library_Api.Resources
{
    public class TagSaveResource
    {
        [Required]
        public string Name { get; set; }
    }
}