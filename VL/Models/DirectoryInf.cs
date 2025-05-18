using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Video_Library_Api.Models
{
    public class DirectoryInf
    {
        public string DirectoryHash { get; set; }
        public string Path { get; set; }
        public string Status { get; set; }
        public ICollection<DirectoryEntry> DirectoryEntries { get; set; }
    }
}