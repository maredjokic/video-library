using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;

namespace Video_Library_Api.Repositories
{
    public interface IVideoTagsRepository
    {
        VideosTags Add(VideosTags videosTags);
        Task<PaginatedList<Tag>> ListPagingAsync(int videoId);
    }
}
