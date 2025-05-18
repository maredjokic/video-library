using Video_Library_Api.Contexts;
using Video_Library_Api.Models;

namespace Video_Library_Api.Repositories
{
    public class VideoGeolocationRepository : BaseRepository, IVideoGeolocationRepository
    {
        public VideoGeolocationRepository(AppDbContext context) : base(context)
        {
        }

        public VideoGeolocation Add(VideoGeolocation videoGeolocation)
        {
            _context.VideoGeolocation.Add(videoGeolocation);
            return videoGeolocation;
        }
    }
}