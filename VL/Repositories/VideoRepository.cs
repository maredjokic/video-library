using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Video_Library_Api.Contexts;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;
using Video_Library_Api.Exceptions;
using Video_Library_Api.Resources;
using Video_Library_Api.Vendor.MotionDSP.Copyright;

namespace Video_Library_Api.Repositories
{
    public class VideoRepository : BaseRepository, IVideoRepository
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VideoRepository(AppDbContext context,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PaginatedList<RelatedVideos>> ListRelatedAsync(Video video, PagingParams pagingParams)
        {
            var relatedTo = _context.RelatedVideos
                .Where(rv => rv.Video2 == video)
                .Include(rv => rv.Video2);

            var relatedFrom = _context.RelatedVideos
                .Where(rv => rv.Video1 == video)
                .Include(rv => rv.Video1);

            var query = relatedTo.Union(relatedFrom)
                .OrderByDescending(rv => rv.Score)
                .Include(rv => rv.Video1)
                .Include(rv => rv.Video2)
                .Where(rv => (rv.Video1.ProcessesLeft == 0) && (rv.Video2.ProcessesLeft == 0));

            //Order by Tag Score first, then order by fpmatch Score
            int count = query.Count();

            List<RelatedVideos> relatedVideos =
                await query.Paging(pagingParams.Page, pagingParams.PageSize)
                .ToListAsync();
            
            return new PaginatedList<RelatedVideos>(relatedVideos, count, pagingParams);
            
        }

        public async Task<PaginatedList<Video>> ListAsync(PagingParams pagingParams, IQueryable<Video> relatedVideos)//moze i da se skrati kod mozda
        {
            int totalCount=0;

            IEnumerable<Video> videos; 
            IQueryable<Video> queryResult;

            if (relatedVideos == null)//if relatedVideos is null then all videos go to search and paging
            {
                if (String.IsNullOrEmpty(pagingParams.PropertySort))
                    pagingParams.PropertySort = "FileName";   //default sort by FileName, ASC is default too

                queryResult = _context.Videos
                    .Include(v => v.VideosTags)
                    .ThenInclude(vt => vt.Tag)
                    .Select(t => t);
            }
            else
            {
                queryResult = relatedVideos;
            }

            String[] tags = pagingParams.Tags.Split(',');

            foreach(string tag in tags)
            {
                if(!String.IsNullOrEmpty(tag))
                {
                    queryResult = queryResult.Where(v => v.VideosTags.Any(g => g.Tag.Name == tag));
                }
            }

            if(pagingParams.FinishedProcessing == false)
            {
                queryResult = queryResult.Where(t => t.ProcessesLeft != 0);
            }
            else
            {
                queryResult = queryResult.Where(t => t.ProcessesLeft == 0);
            }

            if((pagingParams.Latitude >= (-90))
                 && (pagingParams.Latitude <= 90)
                 && (pagingParams.Longitude >= (-180))
                 && (pagingParams.Longitude <= (180)))
            {
                if(pagingParams.Distance==0)
                {
                    queryResult = queryResult
                        .Where(obj => obj.VideoGeolocation!=null)
                        .Where(obj => (obj.VideoGeolocation.FilmedArea.Intersects(new Point(pagingParams.Longitude, pagingParams.Latitude))
                        ||(obj.VideoGeolocation.FilmedArea.Contains(new Point(pagingParams.Longitude, pagingParams.Latitude)))));
                }
                else//if distance is not 0
                {
                    queryResult = queryResult
                        .Where(obj => obj.VideoGeolocation.FilmedArea !=null 
                            && (pagingParams.Distance >= distance(pagingParams.Latitude, pagingParams.Longitude, obj.VideoGeolocation.FilmedArea.Centroid.Y, obj.VideoGeolocation.FilmedArea.Centroid.X, 'K')));
                }

            }

            #region search between
            if (pagingParams.DurationFrom != 0 || pagingParams.DurationTo != double.MaxValue)//duration
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(obj => (Convert.ToDouble(obj.Duration) >= pagingParams.DurationFrom
                            && Convert.ToDouble(obj.Duration) <= pagingParams.DurationTo)));
            }
            if (pagingParams.SizeFrom != 0 || pagingParams.SizeFrom != double.MaxValue)//size
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(obj => (Convert.ToDouble(obj.Size) >= pagingParams.SizeFrom
                            && Convert.ToDouble(obj.Size) <= pagingParams.SizeTo)));
            }
            if (pagingParams.BitRateFrom != 0 || pagingParams.BitRateTo != double.MaxValue)//bitRate
            {

                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(obj => (Convert.ToDouble(obj.BitRate) >= pagingParams.BitRateFrom
                            && Convert.ToDouble(obj.BitRate) <= pagingParams.BitRateTo)));
            }
            if (pagingParams.WidthFrom != 0 || pagingParams.WidthTo != double.MaxValue)//width
            {

                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(obj => (Convert.ToDouble(obj.Width) >= pagingParams.WidthFrom
                            && Convert.ToDouble(obj.Width) <= pagingParams.WidthTo)));
            }
            if (pagingParams.HeightFrom != 0 || pagingParams.HeightTo != double.MaxValue)//height
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(obj => (Convert.ToDouble(obj.Height) >= pagingParams.HeightFrom
                            && Convert.ToDouble(obj.Height) <= pagingParams.HeightTo)));
            }
            #endregion
            
            #region substring search
            if (!String.IsNullOrEmpty(pagingParams.FileName))    //fileName
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(t => t.FileName.ToUpper().Contains(pagingParams.FileName.ToUpper())));
            }
            if (!String.IsNullOrEmpty(pagingParams.FormatLongName))    //formatLongName
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(t => t.FormatLongName.ToUpper().Contains(pagingParams.FormatLongName.ToUpper())));
            }
            if (!String.IsNullOrEmpty(pagingParams.Duration))   //duration
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(t => t.Duration.ToString().ToUpper().Contains(pagingParams.FormatLongName.ToUpper())));
            }
            if (!String.IsNullOrEmpty(pagingParams.Size))   // Size
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(t => t.Size.ToString().ToUpper().Contains(pagingParams.Size.ToUpper())));
            }
            if (!String.IsNullOrEmpty(pagingParams.BitRate))    //bitRate
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(t => t.BitRate.ToString().ToUpper().Contains(pagingParams.BitRate.ToUpper())));
            }
            if (!String.IsNullOrEmpty(pagingParams.Width))    //width
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(t => t.Width.ToString().ToUpper().Contains(pagingParams.Width.ToUpper())));
            }
            if (!String.IsNullOrEmpty(pagingParams.Height))    //height
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(t => t.Height.ToString().ToUpper().Contains(pagingParams.Height.ToUpper())));
            }
            if (!String.IsNullOrEmpty(pagingParams.StreamsJSON))    //streamsJSON
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(t => t.StreamsJSON.ToString().ToUpper().Contains(pagingParams.StreamsJSON.ToUpper())));
            }
            if (!String.IsNullOrEmpty(pagingParams.CodecName))    //CodecName
            {
                queryResult = queryResult.Intersect(_context.Videos
                    .Select(t => t)
                    .Where(t => t.CodecName.ToUpper().Contains(pagingParams.CodecName.ToUpper())));
            }
            #endregion

            
            totalCount = await queryResult.CountAsync();
           

            try
            {

                if (relatedVideos == null)//related videos are sorted before and not need basic sort
                {
                    queryResult = queryResult
                        .OrderBy(pagingParams.PropertySort, pagingParams.SortDirection);
                }

            }
            catch(ArgumentException e)
            {
                throw new PropertyDoesntExistException(e.Message);
            }

            queryResult = queryResult
                .Paging(pagingParams.Page, pagingParams.PageSize);

            videos = await queryResult.ToListAsync();

            return new PaginatedList<Video>(videos, totalCount , pagingParams);
        }

        public async Task<PaginatedList<Video>> ListRelatedByTags(string id, PagingParams pagingParams)
        {
            Video video;

            try
            {
                video = await _context.Videos
                    .Where(v => v.ProcessesLeft == 0)
                    .Where(v => v.Id == id)
                    .Include(v => v.VideosTags)
                    .ThenInclude(vt => vt.Tag)
                    .FirstAsync();
            }
            catch (InvalidOperationException)
            {
                throw new NotFoundException("Video not found");
            }
            
            List<Video> videos = await _context.Videos
                .Where(v => v.ProcessesLeft == 0)
                .Include(v => v.VideosTags)
                .ThenInclude(vt => vt.Tag)
                .Select(v=> v)
                .ToListAsync();

            List<KeyValuePair<int, Video>> keyValuePair = new List<KeyValuePair<int, Video>>();

            foreach(Video v in videos)
            {
                if(v.Id != video.Id)
                {
                    int count = CountIdenticalTags(video.VideosTags.ToList(), v.VideosTags.ToList());
                    if(count != 0)
                    {
                        keyValuePair.Add(new KeyValuePair<int, Video>(count,v));
                    }
                }
            }
            int numOfElements = keyValuePair.Count;
        
            keyValuePair = keyValuePair
                .OrderByDescending(t=>t.Key)
                .Skip((pagingParams.Page - 1) * pagingParams.PageSize)
                .Take(pagingParams.PageSize)
                .ToList();

            List<Video> output = new List<Video>();

            foreach(var pair in keyValuePair)
            {
                output.Add(pair.Value);
            }

            return new PaginatedList<Video>(output,numOfElements,pagingParams);
        }

        public int CountIdenticalTags(List<VideosTags> videosTags1, List<VideosTags> videosTags2)
        {
            List<string> listTags1 = new List<string>();

            List<string> listTags2 = new List<string>();

            int uniqueTagsv1 = 0;
            foreach(VideosTags vt in videosTags1)
            {
                if (!(listTags1.Contains(vt.Tag.Name)))
                { 
                    listTags1.Add(vt.Tag.Name);
                    uniqueTagsv1 ++;
                }
            }

            int uniqueTagsv2 = 0;
            foreach(VideosTags vt in videosTags2)
            {
                if (!(listTags2.Contains(vt.Tag.Name)))
                { 
                    listTags2.Add(vt.Tag.Name);
                    uniqueTagsv2 ++;
                }
            }
            
            int identical = 0;
            foreach(string lt1 in listTags1)
                foreach(string lt2 in listTags2)
                {
                    if (lt1.Equals(lt2)) identical++ ;
                }
                
            return identical;
        }

        public async Task<IEnumerable<Video>> ListAsyncAll()
        {
            return await _context.Videos.ToListAsync();
        }

        public Video Add(Video video)
        {   
            _context.Videos.Add(video);
            return video;
        }

        public VideoGeolocation AddGeolocation(VideoGeolocation videoGeolocation)
        {
            _context.VideoGeolocation.Add(videoGeolocation);
            return videoGeolocation;
        }

        public async Task<Video> FindByIdAsync(string id)
        {
            return await _context.Videos.FindAsync(id);
        }

        public Video Remove(Video video)
        {
            _context.Videos.Remove(video);
            ulong videoID = ulong.Parse(video.Id, System.Globalization.NumberStyles.HexNumber);
            #if linux
            Matcher.RemoveVideo(videoID);
            #endif
            return video;
        }

        public Video Update(Video video)
        {
            _context.Videos.Update(video);
            return video;//required change in return value// some response or true-false
        }

        public async  Task<VideosTags> AddVideosTags(VideosTags videosTags)
        {   
            await _context.VideosTags.AddAsync(videosTags);
            return videosTags;
        }


        public async Task<VideoGeolocation> GetGeolocationAsync(string id)
        {
            try
            {
                return await _context.VideoGeolocation
                    .Where(v => v.VideoId == id)
                    .FirstAsync();
            }
            catch(InvalidOperationException e)
            {
                throw new GeolocationDoesntExistException(e.Message);
            }
        }

        public async Task<PaginatedList<Tag>> ListRelatedByTagsUnique(string id, PagingParams pagingParams)
        {
            try
            {
                Video video = await _context.Videos
                    .Where(v => v.Id == id)
                    .Include(v => v.VideosTags)
                    .ThenInclude(vt => vt.Tag)
                    .FirstAsync();

                HashSet<Tag> uniquetags = new HashSet<Tag>();

                foreach(var tag in video.VideosTags)
                {
                    uniquetags.Add(tag.Tag);
                }
                var tagsList = uniquetags.ToList();

                int count = tagsList.Count;

                List<Tag> list;

                if(pagingParams.SortDirection == "DESC")
                {
                    list = tagsList
                        .OrderByDescending(t => t.Name)
                        .Skip((pagingParams.Page - 1) * pagingParams.PageSize)
                        .Take(pagingParams.PageSize)
                        .ToList();
                }
                else
                {
                    list =  tagsList
                        .OrderBy(t => t.Name)
                        .Skip((pagingParams.Page - 1) * pagingParams.PageSize)
                        .Take(pagingParams.PageSize)
                        .ToList();
                }

                return new PaginatedList<Tag>(list, count, pagingParams);
            }
            catch(InvalidOperationException)
            {
                throw new NotFoundException("Video not found");
            }
        }
    

        private double distance(double lat1, double lon1, double lat2, double lon2, char unit) 
        {
            if ((lat1 == lat2) && (lon1 == lon2)) 
            {
                return 0;
            }
            else 
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K') 
                {
                    dist = dist * 1.609344;
                } 
                else if (unit == 'N') 
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }
        private double deg2rad(double deg) 
        {
            return (deg * Math.PI / 180.0);
        }
        private double rad2deg(double rad) 
        {
            return (rad / Math.PI * 180.0);
        }

        public async Task<int> GetNumberOfVideos()
        {
            return await _context.Videos.CountAsync();
        }

        public void ReloadEntity(Video video)
        {
            var v = _context.Videos.FirstOrDefault(t => t.Id == video.Id);
            _context.Entry(v).Reload();
        }
    }
}
