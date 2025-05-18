using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Video_Library_Api.Models
{
    public class DirectoryEntry
    {
        public string FilePath { get; set; }
        public string DirectoryHash { get; set; }
        public string Status { get; set; }
        public string VideoId { get; set; }
        public DirectoryInf DirectoryInf { get; set; }
    }
}