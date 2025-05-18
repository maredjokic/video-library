using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Video_Library_Api.Exceptions;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Repositories;
using Video_Library_Api.Vendor.SipHash;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using Video_Library_Api.Resources;
using Video_Library_Api.Extensions;

namespace Video_Library_Api.Services
{
    public class VideoService : IVideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly MLService _mlService;
        private readonly FPService _fpService;
        private readonly ThumbnailService _thumbnailService;
        private readonly PreviewService _previewService;
        private readonly TranscodingService _transcodingService;
        private readonly GeoInfoService _geoInfoService;

        public VideoService(
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            MLService mlService,
            FPService fpService,
            ThumbnailService thumbnailService,
            PreviewService previewService,
            TranscodingService transcodingService,
            GeoInfoService geoInfoService)
        {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _mlService = mlService;
            _fpService = fpService;
            _thumbnailService = thumbnailService;
            _previewService = previewService;
            _transcodingService = transcodingService;
            _geoInfoService = geoInfoService;
        }

        public async Task<Video> DeleteAsync(string id)
        {
            Video video = await _videoRepository.FindByIdAsync(id);

            if (video == null)
            {
                throw new NotFoundException("Video not found");
            }

            string storageDir = video.GetDirectory();

            if (!Directory.Exists(storageDir))
            {
                throw new NotFoundException("Video not found in storage!");
            }

            Directory.Delete(storageDir, true);

            DirectoryInfo di = Directory.GetParent(storageDir);
            if(di.GetDirectories().Length == 0)
            {
                di.Delete();
            }

            _videoRepository.Remove(video);
            await _unitOfWork.CompleteAsync();

            return video;
        }

        public async Task<Video> FindAsync(string id)
        {
            var video = await _videoRepository.FindByIdAsync(id);

            if(video == null)
            {
                throw new NotFoundException("Video not found");
            }

            return video;
        }

        public async Task<PaginatedList<Video>> ListAsync(PagingParams pagingParams)
        {
            return await _videoRepository.ListAsync(pagingParams, null);
        }

        public async Task<IEnumerable<Video>> ListAsyncAll()
        {
            return await _videoRepository.ListAsyncAll();
        }

        public async Task<Video> SaveAsync(string videoFilePath, string videoName, string storagePath, bool transcodeMP4, bool fpmatch)
        {
            if(!IsMediaFile(videoName))
                throw new Exception("This file is not video file!");

            string key = _configuration.GetValue<string>("SipHashKey");
            SipHash sipHash = new SipHash(Encoding.ASCII.GetBytes(key));

            var bytes = await File.ReadAllBytesAsync(videoFilePath);
            string hash = sipHash.Compute(bytes).ToString("x16");
            bytes = new byte[] {}; // clean up memory

            if(await _videoRepository.FindByIdAsync(hash) != null)
            {
                throw new DuplicateVideoException();
            }

            string videoDirectory;

            if(storagePath != null && storagePath != "")
            {
                videoDirectory = Path.Combine(
                    _hostingEnvironment.WebRootPath,
                    "storage",
                    "scan",
                    storagePath, 
                    hash.Substring(0, 2),
                    hash.Substring(2, hash.Length - 2));
            }
            else
            {
                videoDirectory = Path.Combine(
                    _hostingEnvironment.WebRootPath,
                    "storage", 
                    hash.Substring(0, 2),
                    hash.Substring(2, hash.Length - 2));
            }

            Directory.CreateDirectory(videoDirectory);

            string filePath = Path.Combine(videoDirectory, videoName);

            try
            {
                if(storagePath == null)
                {
                    File.Move(videoFilePath, filePath);
                }
                else
                {
                    string videoFilePathTxt  = Path.Combine(
                        videoDirectory,
                        "videoPath.txt"
                    );

                    if (!File.Exists(videoFilePathTxt)) 
                    {
                        using (StreamWriter sw = File.CreateText(videoFilePathTxt)) 
                        {
                            sw.WriteLine(videoFilePath);
                        }
                    }
                }
            }
            catch(IOException)
            {
                File.Delete(videoFilePath);
                // file already exists
                throw;
            }
            
            string realVideoPath;
            if(storagePath != null)
            {
                realVideoPath = videoFilePath;
            }
            else
            {
                realVideoPath = Path.Combine(videoDirectory, videoName);
            }

            // ffprobe
            var ffprobeTask =  GenerateJSON("ffprobe",
                $"-v quiet -print_format json -show_format -show_streams \"{realVideoPath}\"",
                videoDirectory, videoName);

            await _unitOfWork.CompleteAsync();
            // klv2json
            #if Linux
            var klvTask = GenerateJSON("klv2json", $"\"{realVideoPath}\"", videoDirectory, videoName);

            await klvTask;
            #endif

            await ffprobeTask;

            Video video = CreateVideo(videoDirectory, hash);
            
            video.StoragePath = storagePath;

            string envNumberOfServices = Environment.GetEnvironmentVariable("NumberOfBackgroundServices");
            int numberOfServices = envNumberOfServices == null ? 0 : int.Parse(envNumberOfServices);

            
            video.ProcessesLeft = 5;//numberOfServices; if ml work then put 6

            if(transcodeMP4 == false)
            {
                video.ProcessesLeft--;
            }

            if(fpmatch == false)
            {
                video.ProcessesLeft--;
            }

            _videoRepository.Add(video);
            await  _unitOfWork.CompleteAsync();

            // start background jobs
            var thumbnailTask = _thumbnailService.Run(video);

            var previewTask = _previewService.Run(video);

            #if Linux
            var videoGeoTask = _geoInfoService.Run(video);

            if(fpmatch != false)
            {
                var fpTask = _fpService.Run(video);
            }
            #endif

            if(transcodeMP4 != false)
            {
                var transcodingTask = _transcodingService.Run(video);
            }

            return video;
        }

        public async Task<Video> UpdateAsync(string id, Video video)
        {
            // just placeholder code
            Video oldVideo = await _videoRepository.FindByIdAsync(id);

            if(oldVideo == null)
            {
                throw new NotFoundException("Video not found");
            }

            var oldName = oldVideo.FileName;
            var extension = Path.GetExtension(oldName);
            oldVideo.FileName = video.FileName + extension;

            _videoRepository.Update(oldVideo);
            await _unitOfWork.CompleteAsync();

            var videoDirectory = Path.Combine(
                _hostingEnvironment.WebRootPath, 
                "storage",
                oldVideo.Id.Substring(0, 2),
                oldVideo.Id.Substring(2, 14));

            var oldVideoPath = Path.Combine(videoDirectory, oldName);
            var newPath = Path.Combine(videoDirectory, video.FileName);

            // rename video file
            File.Move(oldVideoPath, newPath + extension);

            // rename transcoded video file
            File.Move(
                Path.Combine(videoDirectory, Path.GetFileNameWithoutExtension(oldName))
                    + "_transcoded.mp4",
                newPath + "_transcoded.mp4");

            // rename fp file
            File.Move(oldVideoPath + ".fp", newPath + extension + ".fp");

            return oldVideo;
        }

        private async Task GenerateJSON(string command, string args, string videoDirectory, string videoName)
        {
            string videoPath = Path.Combine(videoDirectory, videoName);

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string result = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            string jsonPath = Path.Combine(videoDirectory, $"{command}.json");

            using(var fstream = new FileStream(jsonPath, FileMode.Create))
            {
                await fstream.WriteAsync(Encoding.ASCII.GetBytes(result));
            }
        }
        private Video CreateVideo(string path, string hash)
        {
            string ffprobePath = Path.Combine(path, "ffprobe.json");
            string sjson = File.ReadAllText(ffprobePath);

            if (sjson != null)
            {
                JToken json = null;

                try
                {
                   json = JToken.Parse(sjson);
                }
                catch (JsonReaderException)
                {
                    return null;
                }

                Video video = AddVideoPropreties(json, hash);
                return video;
            }
            else
                return null;
        }

        private Video AddVideoPropreties(JToken json, string hash)
        {
            Video video = new Video();

            video.Id = hash;
            video.FileName = IsNullOrEmpty(json["format"]["filename"]) ? Path.GetFileName((string)json["format"]["filename"]) : "";
            video.StreamsJSON =IsNullOrEmpty(json["streams"].ToString()) ? JsonConvert.SerializeObject(json["streams"]): null;
            video.Duration = IsNullOrEmpty(json["format"]["duration"]) ? (double?)json["format"]["duration"] : null;
            video.FormatLongName = IsNullOrEmpty(json["format"]["format_long_name"]) ? (string)json["format"]["format_long_name"] : "";
            video.Height = null;
            video.Width = null;
            foreach (JToken stream in json["streams"])
            {
                if(IsNullOrEmpty(stream["height"]) && IsNullOrEmpty(stream["width"]))
                {
                    video.Height = (int?)stream["height"];
                    video.Width = (int?)stream["width"];
                    video.CodecName = IsNullOrEmpty(stream["codec_name"]) ? 
                        (string)stream["codec_name"] : null;
                    break;
                }
            }
            video.Size = IsNullOrEmpty(json["format"]["size"]) ? (long?)json["format"]["size"] : null;
            video.BitRate = IsNullOrEmpty(json["format"]["bit_rate"]) ? (long?)json["format"]["bit_rate"] : null;
            if (IsNullOrEmpty(json["format"]["tags"]))
            {
                video.CreationTime = IsNullOrEmpty(json["format"]["tags"]["creation_time"]) ? (DateTime?)DateTime.Parse((string)json["format"]["tags"]["creation_time"]) : null;
            }
            else video.CreationTime = null;
            video.UploadTime = DateTime.Now;
            return video;
        }
        
        //for checking jsons
        private bool IsNullOrEmpty(JToken token)
        {
            return !((token == null) ||
                   (token.Type == JTokenType.Array && !token.HasValues) ||
                   (token.Type == JTokenType.Object && !token.HasValues) ||
                   (token.Type == JTokenType.String && token.ToString() == String.Empty) ||
                   (token.Type == JTokenType.Null));
        }

        //for checking is it video file, hardcoded extension check
        private bool IsMediaFile(string path)
        {
            string[] mediaExtensions = {
              ".WEBM", ".MKV", ".FLV", ".VOB",".OGV", ".OGG", ".DRC", ".GIF",".GIFV",
               ".MNG", ".AVI", ".MTS",".M2TS",".TS", ".MOV", ".QT", ".WMV", ".YUV", ".RM",
               ".RMVB", ".ASF", ".AMV", ".MP4" , ".M4P", ".M4V", ".MPG", ".MP2", ".MPEG",
                ".MPE", ".MPV", ".M2T", ".MPG", ".MPEG", ".M2V", ".M4V", ".SVI", ".3GP",
                ".3G2", ".MXF", ".ROQ", ".NSV", ".FLV", ".F4V", ".F4P", ".F4A", ".F4B"
            };

            return -1 != Array.IndexOf(mediaExtensions, Path.GetExtension(path).ToUpperInvariant());
        }

        public async Task<PaginatedList<Video>> ListRelatedAsync(Video video, PagingParams pagingParams)
        {
            PaginatedList<RelatedVideos> related = await _videoRepository.ListRelatedAsync(video, pagingParams);

            List<Video> videos = new List<Video>();

            foreach (var rv in related.Data)
            {
                if(rv.Video1Id != video.Id)
                {
                    videos.Add(rv.Video1);
                }
                else
                {
                    videos.Add(rv.Video2);
                }
            }

            return new PaginatedList<Video>(videos,related.TotalCount,pagingParams);
        }

        public async Task<VideoGeolocation> GetGeolocationAsync(string id)
        {
           return await _videoRepository.GetGeolocationAsync(id);
        }

        public async Task<VideosTags> AddVideosTags(VideosTags videosTags)
        {   
            await _videoRepository.AddVideosTags(videosTags);
            await _unitOfWork.CompleteAsync();

            return videosTags;
        }

        public async Task<PaginatedList<Video>> ListRelatedByTags(string id, PagingParams pagingParams)
        {
           return await _videoRepository.ListRelatedByTags(id, pagingParams);  
        }

        public async Task<PaginatedList<Tag>> ListRelatedByTagsUnique(string id, PagingParams pagingParams)
        {
            return await _videoRepository.ListRelatedByTagsUnique(id, pagingParams);  
        }

        public async Task<string> GetFFprobeAsync(string id)
        {
            string storagePath = Path.Combine(_hostingEnvironment.WebRootPath, "storage");
            string storageDir = Path.Combine(storagePath, id.Substring(0, 2)); // /xx/
            string videoDirPath = Path.Combine(storageDir, id.Substring(2, id.Length - 2)); 
            string ffprobefile = Path.Combine(videoDirPath, "ffprobe.json");

            return await System.IO.File.ReadAllTextAsync(ffprobefile);//make better formating
        }

        public async Task<int> GetNumberOfVideos()
        {
            return await _videoRepository.GetNumberOfVideos();
        }

        public void ReloadEntity(Video video)
        {
            _videoRepository.ReloadEntity(video);
        }
    }
}