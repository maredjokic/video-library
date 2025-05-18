using System;
using System.Collections.Generic;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Video_Library_Api.Models;
using Video_Library_Api.Resources;
using Video_Library_Api.Services;
using Video_Library_Api.Exceptions;

namespace Video_Library_Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class FeedsController : ControllerBase
    {
        private readonly IFeedsService _feedsService;
        private readonly IMapper _mapper;
        public FeedsController(IFeedsService feedsService,
            IMapper mapper)
        {
            _feedsService = feedsService;
            _mapper = mapper;
        }

        // GET: api/Feeds
        /// <summary>
        /// Get all feeds.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all feeds.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet]
        public ActionResult<IEnumerable<FeedResource>> GetAllAsync()
        {
            try
            {
                IEnumerable<FeedProcessResource> feedProcessResource = _feedsService.GetAll();
                List<FeedResource> feedResources = new List<FeedResource>();
                foreach(FeedProcessResource frp in feedProcessResource)
                {
                    feedResources.Add(frp.FeedResource);
                }
                return Ok(feedResources);
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        // GET: api/Feeds/id
        /// <summary>
        /// Get a feed by id.
        /// </summary>
        /// <param id="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns feed by id.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(500)]
        public ActionResult<FeedResource> GetById(int id)
        {
            try
            {
                return Ok(_feedsService.GetById(id).FeedResource);
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        
        // DELETE: api/Feeds/id
        /// <summary>
        /// Delete feed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Returns the deleted feed.</response>
        /// <response code="404">If tag is not found.</response>
        /// <response code="500">If server error occurred.</response>

        [HttpDelete("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<int> DeleteAsync(int id)
        {
            try
            {
                if(_feedsService.StopStream(id))
                {
                    return id;
                }
                else
                {
                    return -1;
                }
            }
            catch(KeyNotFoundException knfException)
            {
                return StatusCode(404, knfException.Message);
            }
            catch(NotFoundException nfException)
            {
                return StatusCode(404, nfException.Message);
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        // POST: api/Feeds
        /// <summary>
        /// Create new feed.
        /// </summary>
        /// <param name="feedResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns the newly created feed.</response>  
        /// <response code="500">If server error occurred.</response>
        [HttpPost]
        [ProducesResponseType(500)]
        public async Task<ActionResult<FeedResource>> PostAsync([FromBody] FeedResourceInput feedResource)
        {   
            try
            {
                return await _feedsService.StartStream(feedResource);
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        // POST: api/Feeds/startagain
        /// <summary>
        /// Start existing stream again.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Start stream again and return this stream data.</response>  
        /// <response code="404">If stream not exist or stream is active or stream has loop.</response>  
        /// <response code="500">If server error occurred.</response>
        [HttpPost("startagain")]
        [ProducesResponseType(500)]
        public ActionResult<FeedResource> PostStartStreamAgain([FromBody] int id)
        {   
            Console.WriteLine(id);
            try
            {
                return _feedsService.StartStreamAgain(id);
            }
            catch(AlreadyActiveStreamException aase)
            {
                return StatusCode(404, aase.Message);
            }
            catch(NotFoundException nfe)
            {
                return StatusCode(404, nfe.Message);
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }
    }
}