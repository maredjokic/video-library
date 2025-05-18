using System;
using System.Collections.Generic;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Video_Library_Api.Models;
using Video_Library_Api.Resources;
using Video_Library_Api.Services;
using Video_Library_Api.Exceptions;
using System.IO;

namespace Video_Library_Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ScanDirectoryController : ControllerBase
    {
        private readonly IScanDirectoryService _scanDirectoryService;
        private readonly IMapper _mapper;
        private readonly IVideoService _videoService;
        public ScanDirectoryController(IScanDirectoryService scanDirectoryService,
            IMapper mapper,
            IVideoService videoService)
        {
            _scanDirectoryService = scanDirectoryService;
            _mapper = mapper;
            _videoService = videoService;
        }

        // POST: api/ScanDirectory
        /// <summary>
        /// Scan Directory.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        /// <response code="200">Return hash.</response>
        /// <response code="400">Directory does not exist.</response>
        /// <response code="404">Another scan in progress.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<string>> PostAsync([FromBody] DirectoryPathResource directoryPath)
        {   

            Console.WriteLine("Dir path:" + directoryPath.Path);
            try
            {
                if(Directory.Exists(directoryPath.Path))
                {
                    return Ok( await _scanDirectoryService.ScanDirectory(directoryPath.Path,true,false));
                }
                else
                {
                    return StatusCode(404, "Directory does not exist.");
                }
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        // GET api/ScanDirectory
        /// <summary>
        /// Get all scanned directories..
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns all scanned directories.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<DirectoryInfResource>>> GetScannedDirectory()
        {
            try
            {
                List<DirectoryInfResource> dirInfResources = _mapper.Map<List<DirectoryInf>,List<DirectoryInfResource>>(
                    await _scanDirectoryService.GetDirectories());

                foreach(DirectoryInfResource dir in dirInfResources)
                {
                    dir.TotalEntries = await _scanDirectoryService.TotalEntryCount(dir.DirectoryHash);
                    dir.FinishedEntries = await _scanDirectoryService.FinishedEntryCount(dir.DirectoryHash);
                }

                return dirInfResources;
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        //GET api/ScanDirectory/<hash>
        /// <summary>
        /// Get scanned dir information.
        /// </summary>
        ///<param name="hash"></param>
        /// <returns></returns>
        /// <response code="200">Returns information about scanned dir.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{hash}")]
        [ProducesResponseType(500)]
        public async Task<ActionResult<DirectoryInfResource>> GetDirInfo(string hash)
        {
            try
            {
                DirectoryInfResource directoryInfoRecource = 
                    _mapper.Map<DirectoryInf,DirectoryInfResource>(await _scanDirectoryService.GetDirectoryInfoByHash(hash));

                directoryInfoRecource.TotalEntries = await _scanDirectoryService.TotalEntryCount(directoryInfoRecource.DirectoryHash);
                directoryInfoRecource.FinishedEntries = await _scanDirectoryService.FinishedEntryCount(directoryInfoRecource.DirectoryHash);

                return Ok(directoryInfoRecource);
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        //GET api/ScanDirectory/<hash>/details
        /// <summary>
        /// Get informations of video file processing in scanned dir.
        /// </summary>
        ///<param name="hash"></param>
        /// <returns></returns>
        /// <response code="200">Returns informations of video files processing in scanned dir.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpGet("{hash}/details")]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<DirectoryEntryResource>>>  GetVideoFilesProcessingInfo(string hash)
        {
            try
            {
                return Ok(_mapper.Map<List<DirectoryEntry>,List<DirectoryEntryResource>>(await _scanDirectoryService.GetEntriesByHash(hash)));
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }


        //POST api/ScanDirectory/pause
        /// <summary>
        /// Pause scanning.
        /// </summary>
        ///<param name="directoryPath"></param>
        /// <returns></returns>
        /// <response code="200">Return hash for this directory.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpPost("pause")]
        [ProducesResponseType(500)]
        public ActionResult<string> PauseDirScanning([FromBody] DirectoryPathResource directoryPath)
        {
            try
            {
                return Ok(_scanDirectoryService.PauseScanning(directoryPath.Path));
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                return StatusCode(500, exception);
            }       
        }

        //POST api/ScanDirectory/resume
        /// <summary>
        /// Resume scanning.
        /// </summary>
        ///<param name="directoryPath"></param>
        /// <returns></returns>
        /// <response code="200">Return hash for this directory.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpPost("resume")]
        [ProducesResponseType(500)]
        public async  Task<ActionResult<string>> ResumeDirScanning([FromBody] DirectoryPathResource directoryPath)
        {   
            try
            {
                return Ok(await _scanDirectoryService.ResumeScanning(directoryPath.Path));
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                return StatusCode(500, exception);
            }
        }

        //POST api/ScanDirectory/cleanup
        /// <summary>
        /// Check the directory again, delete video information if the video bas been deleted, or add video information if a new video has been added.
        /// </summary>
        ///<param name="directoryPath"></param>
        /// <returns></returns>
        /// <response code="200">Return hash for this directory.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpPost("cleanup")]
        [ProducesResponseType(500)]
        public async Task<ActionResult<string>> DirectoryCleanup([FromBody] DirectoryPathResource directoryPath)
        {       
            try
            {
                directoryPath.Path = directoryPath.Path.Replace("%2F", "/");
                Console.WriteLine("cleanup started" + directoryPath.Path);
                return Ok( await _scanDirectoryService.Cleanup(directoryPath.Path));
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception);
                return StatusCode(500, exception);
            }
        }

        //DELETE: api/ScanDirectory/<path>
        /// <summary>
        /// Stop processing and delete data for videos in dir.
        /// </summary>
        ///<param name="hash"></param>
        /// <returns></returns>
        /// <response code="200">Return hash for this directory.</response>
        /// <response code="500">If server error occurred.</response>
        [HttpDelete("{hash}")]
        [ProducesResponseType(500)]
        public async Task<ActionResult<string>> DeleteScanDir(string hash)
        {   
            try
            {
                DirectoryInf directoryInf = await _scanDirectoryService.GetDirectoryInfoByHash(hash);

                List<DirectoryEntry> directoryEntries = await _scanDirectoryService.GetEntriesByHash(hash);
                
                foreach(DirectoryEntry de in directoryEntries)
                {
                    try
                    {
                        if(de.VideoId != null)
                            await _videoService.DeleteAsync(de.VideoId);
                    }
                    catch(Exception){}

                    await _scanDirectoryService.DeleteDirectoryEntryAsync(de);
                }

                await _scanDirectoryService.DeleteDirectoryInfosAsync(directoryInf);

                return hash;
            }
            catch(Exception exception)
            {
                return StatusCode(500, exception);
            }
        }
    }
}