using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Resources;

namespace Video_Library_Api.Repositories
{
    public class MockVideoRepository : IVideoRepository
    {
        private List<Video> _videolist;

        public MockVideoRepository()
        {
            _videolist = new List<Video>()
            {
            new Video(){ Id = "1", FileName =  "Video1", BitRate = 0, StreamsJSON =" ", Duration = 0, FormatLongName = " ", Height = 0, Width = 0, Size = 0 },
            new Video(){ Id = "2", FileName =  "Video2", BitRate = 0, StreamsJSON =" ", Duration = 0, FormatLongName = " ", Height = 0, Width = 0, Size = 0 },
            new Video(){ Id = "3", FileName =  "Video3", BitRate = 0, StreamsJSON = " ", Duration = 0, FormatLongName = " ", Height = 0, Width = 0, Size = 0 }
            };
        }

        public Video Add(Video video)
        {
            video.Id = _videolist.Max(v => v.Id) + 1;
            _videolist.Add(video);
            return video;
        }

        public Video Remove(Video video)
        {

            string id = video.Id;
            Video vid = _videolist.FirstOrDefault(v => v.Id == id);
                if (video != null)
            {
                _videolist.Remove(video);
            }
            return video;
        }

        public Task<Video> FindByIdAsync(string id)
        {
            //Video video =  _videolist.FirstOrDefault(p => p.Id == id); ;
            //return video;
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Video>> ListAsyncAll()
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<Video>> ListAsync(PagingParams pagingParams, IQueryable<Video> relatedVideos)
        {
            throw new NotImplementedException();
        }
        
        public Video Update(Video video)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<RelatedVideos>> ListRelatedAsync(Video video,PagingParams pagingParams)
        {
            throw new NotImplementedException();
        }
        public Task<VideoGeolocation> GetGeolocationAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<VideosTags> AddVideosTags(VideosTags videosTags)
        {
            throw new NotImplementedException();
        }

        public VideoGeolocation AddGeolocation(VideoGeolocation videoGeolocation)
        {
             throw new NotImplementedException();
        }
        public Task<PaginatedList<Video>> ListRelatedByTags(string id, PagingParams pagingParams)
        {
            throw new NotImplementedException();
        }
        public Task<PaginatedList<Tag>> ListRelatedByTagsUnique(string id, PagingParams pagingParams)
        {
            throw new NotImplementedException();
        }

        public  Task<int> GetNumberOfVideos()
        {
            throw new NotImplementedException();
        }
        void IVideoRepository.ReloadEntity(Video video)
        {
            throw new NotImplementedException();
        }
    }
}
