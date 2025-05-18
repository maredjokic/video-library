using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Video_Library_Api.Models
{
    public class VideoGeolocation
    {
        [MaxLength(16)]
        public string VideoId { get; set; }
        [Column(TypeName="geography")]
        public LineString CameraLine { get; set; }

        [Column(TypeName="geography")]
        public Polygon FilmedArea { get; set; }
        public Video Video { get; set; }
    }
}