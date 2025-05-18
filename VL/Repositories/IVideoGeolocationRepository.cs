using Video_Library_Api.Models;

namespace Video_Library_Api.Repositories
{
    public interface IVideoGeolocationRepository
    {
        VideoGeolocation Add(VideoGeolocation videoGeolocation);
    }
}