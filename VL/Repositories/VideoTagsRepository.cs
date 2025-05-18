using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Video_Library_Api.Contexts;
using Video_Library_Api.Models;
using Video_Library_Api.Resources;
using Microsoft.AspNetCore.Mvc;
using Video_Library_Api.Paging;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Video_Library_Api.Repositories
{
    public class VideoTagsRepository : BaseRepository, IVideoTagsRepository
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public VideoTagsRepository(AppDbContext context,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public VideosTags Add(VideosTags videosTags)
        {
            _context.VideosTags.Add(videosTags);
            return videosTags;
        }

        public Task<PaginatedList<Tag>> ListPagingAsync(int videoId)
        {
            throw new NotImplementedException();
        }
    }
}

