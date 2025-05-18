using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Video_Library_Api.Paging;
using Video_Library_Api.Resources;
using Video_Library_Api.Exceptions;

namespace Video_Library_Api.Repositories
{
    public class TagRepository : BaseRepository, ITagRepository
    {
        public TagRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Tag>> ListAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public Tag Add(Tag tag)
        {
            _context.Tags.Add(tag);
            return tag;
        }

        public async Task<Tag> FindByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public Tag Remove(Tag tag)
        {
            _context.Tags.Remove(tag);
            return tag;
        }

        public Tag Update(Tag tag)
        {
            _context.Tags.Update(tag);
            return tag;//required change in return value// some response or true-false
        }

        public async Task<Tag> FindByNameAsync(string name)
        {
            return await _context.Tags
                .AsNoTracking()
                .Where(c => c.Name == name)
                .FirstOrDefaultAsync();
        }

        public async Task<VideosTagsResource> DeleteVideoTagsAsync(string videoId,string name, int? from, int? to)
        {
            Tag tag;
            Video video;
            
            try
            {
                tag = _context.Tags.Where(t => t.Name == name).First();
            }
            catch(InvalidOperationException)
            {
                throw new NotFoundException("Tag not found.");
            }

            try
            {
                video = await _context.Videos
                    .Where(v => v.Id == videoId)
                    .Include(t => t.VideosTags)
                    .ThenInclude(t => t.Tag)
                    .FirstAsync();
            }
            catch(InvalidOperationException)
            {
                throw new NotFoundException("Video not found.");
            }

            int tagNum = video.VideosTags.Count;

            int count = 0;

            if(from == null || to == null)
            {    
                for(int i = tagNum-1; i > -1 ; i--)//go from the end
                {
                    if(video.VideosTags.ElementAt(i).Tag.Name == name)
                    {               
                        video.VideosTags.Remove(video.VideosTags.ElementAt(i));
                        count++;
                    }
                }
            }
            else
            {
                for(int i = tagNum-1; i > -1 ; i--)//go from the end
                {
                    if(video.VideosTags.ElementAt(i).Tag.Name == name && 
                            video.VideosTags.ElementAt(i).From >= from &&
                            video.VideosTags.ElementAt(i).To <= to )
                    {  
                        video.VideosTags.Remove(video.VideosTags.ElementAt(i));
                        count++;
                    }
                }
            }

            if(count == 0)
            {
                if(from == null && to == null)
                {
                    throw new NotFoundException("Tag doesn't exist in this video.");
                }
                
                throw new NotFoundException("Tag doesn't exist in this video or in requested interval.");
            }

            if(from == null && to == null)
            {
                from = 0;
                to = 0;
            }

            int resourceFrom = (int)from;
            int resourceTo = (int)to;

            VideosTagsResource videosTagsResource = new VideosTagsResource()
            {
                TagName = name,
                From = resourceFrom,
                To = resourceTo
            };
            
            return videosTagsResource;
        }

        public async Task<PaginatedList<VideosTags>> ListAsyncByVideoId(string videoId, PagingParams pagingParams)
        {
            try
            {
                var query =_context.VideosTags
                    .Select(t => t)
                    .Where(c => c.VideoId == videoId);

                int count = query.Count();

                if(pagingParams.PropertySort == "From" || pagingParams.PropertySort == "To")
                {
                    query = query.OrderBy(pagingParams.PropertySort, pagingParams.SortDirection);
                }
                else
                {   
                    if(pagingParams.SortDirection == "DESC")
                    {
                        query = query.OrderByDescending(t => t.Tag.Name);
                    }
                    else
                    {
                        query = query.OrderBy(t => t.Tag.Name);
                    }
                }

                query = query.Paging(pagingParams.Page, pagingParams.PageSize).Include(t => t.Tag);
                
                IEnumerable<VideosTags> videosTagsList = await query.ToListAsync();

                return new PaginatedList<VideosTags>(videosTagsList,count,pagingParams);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message + e.Data);
                return null;
            }
        }
    }
}
