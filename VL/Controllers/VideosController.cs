using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Services;
using Video_Library_Api.Exceptions;
using Video_Library_Api.Paging;
using Video_Library_Api.Resources;
using System.IO;
using System;
using Microsoft.Extensions.Configuration;
using Video_Library_Api.Repositories;
using Video_Library_Api.Extensions;

namespace Video_Library_Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly SeedDatabase _seedDatabase;

        public VideosController(
            IVideoService videoService, 
            IMapper mapper,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            SeedDatabase seedDatabase,
            ITagService tagService)
        {
            _videoService = videoService;
            _mapper = mapper;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _seedDatabase = seedDatabase;
            _tagService = tagService;
        }


        //GET: api/Videos/count
        /// <summary>
        /// Get number of videos.
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        /// <response code="200">Returns number of videos.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("count")]
        [ProducesResponseType(500)]
        public async Task<ActionResult<int>> GetNumberOfVideosAsync()
        {
            try
            {
                return await _videoService.GetNumberOfVideos();
            }
            catch(Exception)
            {   
                return StatusCode(500, "Something went wrong.");
            }
        }

        // GET: api/Videos
        /// <summary>
        /// Get all videos.
        /// </summary>
        /// <param name="pagingParams"></param>
        /// <returns></returns>
        /// <response code="200">Returns paginated list of videos with paging metadata.</response>
        /// <response code="400">If property for sort doesn't exists.</response>
        /// <response code="404">if videos are not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedList<VideoResource>>> ListAsync([FromQuery] PagingParams pagingParams)
        {
            try
            {
                PaginatedList<Video> list = await _videoService.ListAsync(pagingParams);
                return Ok(new PaginatedList<VideoResource>(
                    _mapper.Map<IEnumerable<Video>,IEnumerable<VideoResource>>(list.Data), list.TotalCount, pagingParams));

            }
            catch (PropertyDoesntExistException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception)
            {   
                return StatusCode(500, "Something went wrong.");
            }
        }

        // GET: api/Videos/id
        /// <summary>
        /// Get a video by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns video by id.</response>
        /// <response code="404">If video is not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<VideoResource>> GetAsync(string id)
        {
            try
            {
                Video video =  await _videoService.FindAsync(id);
                return _mapper.Map<VideoResource>(video);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }

        // GET: api/Videos/id/relatedfp
        /// <summary>
        /// Get related videos by fpMatch score.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pagingParams"></param>
        /// <returns></returns>
        /// <response code="200">Returns related videos by fpMatch.</response>
        /// <response code="404">If video is not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{id}/relatedfp")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedList<VideoResource>>> ListRelatedFp(string id, [FromQuery] PagingParams pagingParams)
        {
            try
            {
                var video = await _videoService.FindAsync(id);
                var related = await _videoService.ListRelatedAsync(video, pagingParams);
                
                return Ok(new PaginatedList<VideoResource>( 
                    _mapper.Map<IEnumerable<VideoResource>>(related.Data), related.TotalCount,pagingParams));
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }

        // GET: api/Videos/id/tags/relatedtags
        /// <summary>
        /// Get related videos with same tags.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pagingParams"></param>
        /// <returns></returns>
        /// <response code="200">Returns related videos by tags.</response>
        /// <response code="404">If video is not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{id}/relatedtags")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedList<VideoResource>>> ListRelatedByTags(string id, [FromQuery] PagingParams pagingParams)
        {
            try
            {   
                PaginatedList<Video> list = await _videoService.ListRelatedByTags(id, pagingParams);

                return Ok(new PaginatedList<VideoResource>( 
                    _mapper.Map<IEnumerable<VideoResource>>(list.Data), list.TotalCount, pagingParams));
            
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }


        // GET: api/Videos/processing
        /// <summary>
        /// Get videos which are currently being processed.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns currently processing videos.</response>
        /// <response code="404">If videos are not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("processing")]
        public async Task<ActionResult<PaginatedList<VideoResource>>> ListProcessing([FromQuery] PagingParams pagingParams)
        {
            try
            {
                pagingParams.FinishedProcessing = false;
                pagingParams.PropertySort = "ProcessesLeft";
                return await ListAsync(pagingParams);
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }

        }
        

        // GET: api/values/id/geolocation/lat-long
        /// <summary>
        /// Get geolocation for video.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns filmed area and camera line for video.</response>
        /// <response code="404">If video doesn't exists.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{id}/geolocation/lat-long")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<VideoGeolocationResource>> GetGeolocationLatLongAsync(string id)
        {
            try
            {
                VideoGeolocation videoGeolocation = await _videoService.GetGeolocationAsync(id);

                if(videoGeolocation != null)
                {
                    //PostGis long/lat
                    if( videoGeolocation.CameraLine != null)
                    {
                        double pom;
                        foreach(var coordinate in videoGeolocation.CameraLine.Coordinates)
                        {
                            pom = coordinate.X;
                            coordinate.X = coordinate.Y;
                            coordinate.Y = pom;
                        }
                    }

                    if( videoGeolocation.FilmedArea != null)
                    {
                        double pom;
                        foreach(var coordinate in videoGeolocation.FilmedArea.Coordinates)
                        {
                            pom = coordinate.X;
                            coordinate.X = coordinate.Y;
                            coordinate.Y = pom;
                        }
                    }
                }

                return Ok(_mapper.Map<VideoGeolocationResource>(videoGeolocation));
            }
            catch(GeolocationDoesntExistException)
            {
                return NotFound("Geolocation doesn't exist");
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error {e.Message}");
            }
        }

        // GET: api/values/id/geolocation/long-lat
        /// <summary>
        /// Get geolocation for video.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns filmed area and camera line for video.</response>
        /// <response code="404">If video doesn't exists.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{id}/geolocation/long-lat")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<VideoGeolocationResource>> GetGeolocationLongLatAsync(string id)
        {
            try
            {
                VideoGeolocation videoGeolocation = await _videoService.GetGeolocationAsync(id);

                return Ok(_mapper.Map<VideoGeolocationResource>(videoGeolocation));
            }
            catch(GeolocationDoesntExistException)
            {
                return NotFound("Geolocation doesn't exist");
            }
            catch(NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error {e.Message}");
            }
        }

        // POST: api/Videos
        /// <summary>
        /// Upload new video.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns uploaded video.</response>
        /// <response code="400">If no video file is sent, video already exists or video file is empty.</response>
        /// <response code="500">If server error occurred.</response>
        /// <remarks>
        /// 
        /// Send video in form data field.
        /// Only accepts one video.
        /// 
        /// </remarks>
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = (long) 1e10)] // 10GB limit on file size
        [RequestSizeLimit((long) 1e10)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<VideoResource>> PostAsync()
        {
            try
            {
                if(Request.Form.Files.Count() == 0)
                {
                    return BadRequest("No video file sent");
                }

                var file = Request.Form.Files[0];

                string tmpFileName = Path.GetTempFileName();

                if (file.Length > 0)
                {
                    using(var stream = new FileStream(tmpFileName, FileMode.Open))
                    {
                        await file.CopyToAsync(stream);
                    }
                    var video = await _videoService.SaveAsync(tmpFileName, file.FileName, null, true, true);
                    return _mapper.Map<Video, VideoResource>(video);
                }
                else
                {
                    return BadRequest("Empty file sent");
                }
            }
            catch(DuplicateVideoException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error {e.Message}");
            }
        }

        // POST api/videos/id/tags
        /// <summary>
        /// Add new tag to video.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tagResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns updated video.</response>
        /// <response code="404">If tag in that interval is already added.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpPost("{id}/tags")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<VideosTagsResource>> PutTagAsync(string id,[FromBody] VideosTagsResource tagResource)
        {
            try
            {
                Tag tagFind = await _tagService.FindByNameAsync(tagResource.TagName);
                if(tagFind == null)
                {
                    tagFind = new Tag(){ Name = tagResource.TagName };
                    tagFind = await _tagService.SaveAsync(tagFind);
                    await _unitOfWork.CompleteAsync();
                }

                Video video = await _videoService.FindAsync(id);

                if((tagResource.From > tagResource.To) ||
                    (tagResource.From < 0))
                {
                    return BadRequest("Invalid interval");
                }

                if(tagResource.To > video.Duration)
                {
                    return BadRequest("\"To\" is greater than video duration");
                }

                if(video == null)
                    return null;

                VideosTags videosTags = new VideosTags()
                {
                    VideoId = id,
                    TagId = tagFind.Id,
                    From = tagResource.From,
                    To = tagResource.To,
                    Video = video
                };

                await _videoService.AddVideosTags(videosTags);
                return Ok(tagResource);
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine("Tag exists in this time period." +  e.Message);
                return BadRequest("Tag exists in this time period." +  e.Message);
            }
        }

        // GET api/videos/id/tags
        /// <summary>
        /// Get all tags associated with video.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pagingParams"></param>
        /// <returns></returns>
        /// <response code="200">Returns paginated list of tags associated with video.</response>
        /// <response code="404">If video is not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{id}/tags")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedList<VideosTagsResource>>> ListAsyncTags(string id, [FromQuery] PagingParams pagingParams)
        {
            try
            {
                var videosTags = await _tagService.ListAsyncByVideoId(id, pagingParams);

                return Ok(new PaginatedList<VideosTagsResource>( 
                    _mapper.Map<IEnumerable<VideosTagsResource>>(videosTags.Data), videosTags.TotalCount , pagingParams));
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Data + e.Message);
                return StatusCode(500, "Something went wrong.");
            }
        }

        // PUT api/Videos/id
        /// <summary>
        /// Rename video.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videoResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns renamed video.</response>
        /// <response code="404">If video is not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(404)] 
        [ProducesResponseType(500)] 
        public async Task<ActionResult<Video>> PutAsync(string id, [FromForm] VideoSaveResource videoResource)
        {
            try
            {
                Video video = _mapper.Map<Video>(videoResource);
                return await _videoService.UpdateAsync(id, video);
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }

        // DELETE api/Videos/id
        /// <summary>
        /// Delete video.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns deleted video.</response>
        /// <response code="404">If video is not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)] 
        public async Task<ActionResult<Video>> DeleteVideo(string id)
        {
            try
            {
                return await _videoService.DeleteAsync(id);
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }

        // DELETE api/Videos/id/tags
        /// <summary>
        /// Delete tag from video.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videosTagsResource"></param>
        /// <returns></returns>
        /// <response code="200">Return deleted tag.</response>
        /// <response code="404">If video or tag is not found or tag doesn't exist in this video or in requested interval.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpDelete("{id}/tags")]
        [ProducesResponseType(404)] 
        [ProducesResponseType(500)] 
        public async Task<ActionResult<VideosTagsResource>> DeleteVideoTagsAsync(string id, [FromForm] VideosTagsResource videosTagsResource)
        {
            try
            {
                return await _tagService.DeleteVideoTagsAsync(id, videosTagsResource.TagName, videosTagsResource.From, videosTagsResource.To);
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }

        // GET api/videos/id/tags/unique
        /// <summary>
        /// Get all unique tags associated with video.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pagingParams"></param>
        /// <returns></returns>
        /// <response code="200">Returns paginated list of unique tags associated with video.</response>
        /// <response code="404">If video is not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{id}/tags/unique")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PaginatedList<VideosTagsResource>>> ListAsyncTagsUnique(string id, [FromQuery] PagingParams pagingParams)
        {
            try
            {
                var videosTags = await _videoService.ListRelatedByTagsUnique(id, pagingParams);

                return Ok(new PaginatedList<TagResource>( 
                     _mapper.Map<IEnumerable<TagResource>>(videosTags.Data), videosTags.TotalCount , pagingParams));
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Data + e.Message);
                return StatusCode(500, "Something went wrong.");
            }
        }


          // DELETE api/Videos/id/tags/unique
        /// <summary>
        /// Delete all occurrences of the tag from video.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videosTagsResource"></param>
        /// <returns></returns>
        /// <response code="200">Return deleted tag.</response>
        /// <response code="404">If video or tag is not found or tag doesn't exist in video.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpDelete("{id}/tags/unique")]
        [ProducesResponseType(404)] 
        [ProducesResponseType(500)] 
        public async Task<ActionResult<VideosTagsDeleteResource>> DeleteVideoTagsUniqueAsync(string id, [FromForm] VideosTagsDeleteResource videosTagsResource)
        {
            try
            {
                await _tagService.DeleteVideoTagsAsync(id, videosTagsResource.TagName, null, null);
                return videosTagsResource;
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }


        // Get api/Videos/id/ffprob
        /// <summary>
        /// Get text from ffprobe file.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Return text from ffprobe file.</response>
        /// <response code="404">If ffprobe file is not found or ffprobe file doesn't exist in video.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{id}/ffprobe")]
        public async Task<ActionResult<string>> GetffprobeFile(string id)
        {
            try
            {
                return await _videoService.GetFFprobeAsync(id);
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong");
            }
        }


    }
}