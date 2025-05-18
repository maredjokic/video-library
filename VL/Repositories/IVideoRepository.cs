using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Resources;

namespace Video_Library_Api.Repositories
{
    public interface IVideoRepository
    {
        Task<PaginatedList<Video>> ListAsync(PagingParams pagingParams, IQueryable<Video> relatedVideos);
        Video Add(Video video);
        Task<Video> FindByIdAsync(string id);
        Task<PaginatedList<RelatedVideos>> ListRelatedAsync(Video video,PagingParams pagingParams);
        Video Update(Video video);
        Video Remove(Video video);
        Task<IEnumerable<Video>> ListAsyncAll();
        Task<VideoGeolocation> GetGeolocationAsync(string id);
        Task<VideosTags> AddVideosTags(VideosTags videosTags);
        VideoGeolocation AddGeolocation(VideoGeolocation videoGeolocation);
        Task<PaginatedList<Video>> ListRelatedByTags(string id, PagingParams pagingParams);
        Task<PaginatedList<Tag>> ListRelatedByTagsUnique(string id, PagingParams pagingParams);
        Task<int> GetNumberOfVideos();
        void ReloadEntity(Video video);
    }
}
