using System.Collections.Generic;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Resources;

namespace Video_Library_Api.Services
{
    public interface ITagService
    {
        Task<Tag> FindAsync(int id);
        Task<IEnumerable<Tag>> ListAsync();
        Task<Tag> SaveAsync(Tag tag);
        Task<Tag> UpdateAsync(string name, Tag tag);
        Task<Tag> DeleteAsync(string name);
        Task<Tag> FindByNameAsync(string name);
        Task<PaginatedList<VideosTags>> ListAsyncByVideoId(string videoId, PagingParams pagingParams);
        Task<VideosTagsResource> DeleteVideoTagsAsync(string videoId,string name, int? from, int? to);
    }
}