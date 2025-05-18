using System.Collections.Generic;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Resources;

namespace Video_Library_Api.Services
{
    public interface IVideoService
    {
        Task<Video> FindAsync(string id);
        Task<PaginatedList<Video>> ListAsync(PagingParams pagingParams);
        Task<PaginatedList<Video>> ListRelatedAsync(Video video, PagingParams pagingParams);
        Task<Video> SaveAsync(string videoFilePath, string videoName, string storagePath, bool transcodeMP4, bool fpmatch);
        Task<Video> UpdateAsync(string id, Video video);
        Task<Video> DeleteAsync(string id);
        Task<IEnumerable<Video>> ListAsyncAll();
        Task<VideoGeolocation> GetGeolocationAsync(string id);
        Task<VideosTags> AddVideosTags(VideosTags videosTags);
        Task<PaginatedList<Video>> ListRelatedByTags(string id, PagingParams pagingParams);
        Task<PaginatedList<Tag>> ListRelatedByTagsUnique(string id, PagingParams pagingParams);
        Task<string> GetFFprobeAsync(string id);
        Task<int> GetNumberOfVideos();
        void ReloadEntity(Video video);
        
    }
}