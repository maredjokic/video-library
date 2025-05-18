using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Resources;


namespace Video_Library_Api.Repositories
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> ListAsync();
        Tag Add(Tag tag);
        Task<Tag> FindByIdAsync(int id);
        Tag Update(Tag tag);
        Tag Remove(Tag tag);
        Task<Tag> FindByNameAsync(string name);
        Task<PaginatedList<VideosTags>> ListAsyncByVideoId(string videoId, PagingParams pagingParams);
        Task<VideosTagsResource> DeleteVideoTagsAsync(string videoId,string name, int? from, int? to);
    }
}
