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
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;

        public TagsController(ITagService tagService, IMapper mapper)
        {
            _tagService = tagService;
            _mapper = mapper;
        }

        // GET: api/Tags
        /// <summary>
        /// Get all tags.
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all tags.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagResource>>> GetAsync()
        {
            try
            {
                var tags = await _tagService.ListAsync();
                return Ok(_mapper.Map<IEnumerable<Tag>, IEnumerable<TagResource>>(tags));
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }

        // GET: api/Tags/name
        /// <summary>
        /// Get a tag by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <response code="200">Returns tag by name.</response>
        /// <response code="404">If tag is not found.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{name}")]
        [ProducesResponseType(404)]
        public async Task<ActionResult<TagResource>> GetAsync(string name)
        {
            try
            {
                return _mapper.Map<TagResource>(
                    await _tagService.FindByNameAsync(name));
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

        // POST: api/Tags
        /// <summary>
        /// Add new tag.
        /// </summary>
        /// <param name="tagResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns the newly created tag.</response>  
        /// <response code="400">If tag name already exists</response>
        /// <response code="500">If server error occurred.</response>
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TagResource>> PostAsync([FromForm] TagSaveResource tagResource)
        {
            Tag newTag = _mapper.Map<Tag>(tagResource);
            
            try
            {
                newTag = await _tagService.SaveAsync(newTag);
                return _mapper.Map<TagResource>(newTag);
            }
            catch(TagExistsException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong.");
            }
        }

        // PUT: api/Tags/name
        /// <summary>
        /// Rename tag.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tagResource"></param>
        /// <returns></returns>
        /// <response code="200">Returns the updated tag.</response>
        /// <response code="400">If tag is not user added.</response>
        /// <response code="404">If tag is not found.</response>  
        /// <response code="500">If server error occurred.</response>
        [HttpPut("{name}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TagResource>> PutAsync(string name, [FromForm] TagSaveResource tagResource)
        {
            try
            {
                var tag = _mapper.Map<Tag>(tagResource);
                tag = await _tagService.UpdateAsync(name, tag);

                return _mapper.Map<TagResource>(tag);
            }
            catch(NotUserAddedTagException e)
            {
                return BadRequest(e.Message);
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong with updating tag");
            }
        }

        // DELETE: api/Tags/name
        /// <summary>
        /// Delete tag.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <response code="200">Returns the deleted tag.</response>
        /// <response code="400">If tag is not user added.</response>
        /// <response code="404">If tag is not found.</response>  
        /// <response code="500">If server error occurred.</response>

        [HttpDelete("{name}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<TagResource>> DeleteAsync(string name)
        {
            try
            {
                return _mapper.Map<TagResource>(
                    await _tagService.DeleteAsync(name));
            }
            catch(NotUserAddedTagException e)
            {
                return BadRequest(e.Message);
            }
            catch(NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch(Exception)
            {
                return StatusCode(500, "Something went wrong with deleting tag");
            }
        }

    }
}