using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Video_Library_Api.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public bool UserAdded { get; set; }


        public IEnumerable<VideosTags> VideosTags { get; set; }
    }
}