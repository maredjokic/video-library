using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace Video_Library_Api.Models
{
    public class VideoGeolocationResource
    {
        [MaxLength(16)]
        public string VideoId { get; set; }
        public LineString CameraLine { get; set; }
        public Polygon FilmedArea { get; set; }
    }
}