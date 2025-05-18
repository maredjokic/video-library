using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Video_Library_Api.Extensions;
using Video_Library_Api.Models;
using Video_Library_Api.Repositories;
using Video_Library_Api.Services.Semaphores;
using Video_Library_Api.Services.Strategies;

namespace Video_Library_Api.Services
{
    public class MLService : BackgroundService
    {
        private readonly IMLStrategy _mlStrategy;

        public MLService(
            IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundService> logger,
            IConfiguration configuration,
            BackgroundGPUServiceSemaphore semaphore,
            IMLStrategy mLStrategy)
            : base(scopeFactory, logger, semaphore)
        { 
            _mlStrategy = mLStrategy;
        }

        private class DetectedTag
        {
            public string name = string.Empty;
            public int[] interval = new int[] {0, 0};
        }
        protected async override Task StartAsync(Video video, IServiceScope scope)
        {
            string hash = video.Id;
            string videoPath = video.GetPath();

            Console.WriteLine($"[{hash}]: ML started");
            _logger.LogInformation($"[{hash}]: ML started");
            
            var _unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            var _videoTagsRepository = scope.ServiceProvider.GetService<IVideoTagsRepository>();
            var _tagRepository = scope.ServiceProvider.GetService<ITagRepository>();
            var _hostingEnvironment = scope.ServiceProvider
                .GetService<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
        
            string jsonString = string.Empty;

            try
            {
                jsonString = await _mlStrategy.GetJsonAsync(video);
            }
            catch (System.Exception)
            {
                _logger.LogInformation($"[{hash}]: ML crashed");
                return;
            }

            if(jsonString != "No detections were made\n" && IsValidJson(jsonString))
            {
                var detectedTags = JsonConvert.DeserializeObject<DetectedTag[]>(jsonString);
                foreach(var detectedTag in detectedTags)
                {
                    Tag tag = await _tagRepository.FindByNameAsync(detectedTag.name);
                    var from = detectedTag.interval[0];
                    var to = detectedTag.interval[1];


                    VideosTags videosTags = new VideosTags();

                    videosTags.VideoId = hash;
                    videosTags.TagId = tag.Id;
                    videosTags.From = from;
                    videosTags.To = to;

                    _videoTagsRepository.Add(videosTags);
                }
            }
            
            Console.WriteLine($"[{hash}]: ML finished");
            _logger.LogInformation($"[{hash}]: ML finished");
            await _unitOfWork.CompleteAsync();
        }
        private bool IsValidJson(string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                (value.StartsWith("[") && value.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }
    }
}