using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Video_Library_Api.Extensions;
using Video_Library_Api.Models;
using Video_Library_Api.Repositories;
using Video_Library_Api.Services.Semaphores;

namespace Video_Library_Api.Services
{
    public class ThumbnailService : BackgroundService
    {
        public ThumbnailService(
            IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundService> logger, 
            BackgroundCPUServiceSemaphore semaphore)
            : base(scopeFactory, logger, semaphore)
        {}

        protected override Task StartAsync(Video video, IServiceScope scope)
        {
            string hash = video.Id;
            string videoDirectory = video.GetDirectory();
            string videoName = video.FileName;
            double? duration = video.Duration;
            string videoFilePath = video.GetVideoFilePath();

            return Task.Run(() => 
            {
                SaveThumbnail(videoFilePath, hash, videoDirectory, videoName, duration);
            });
        }

        private void SaveThumbnail(string videoFilePath, string hash, string videoDirectory, string videoName, double? duration)
        {
            Console.WriteLine($"[{hash}]: Thumbnail generation started");
            _logger.LogInformation($"[{hash}]: Thumbnail generation started");

            try
            {
                if(duration == null)
                {
                    duration = 0;
                }
                else
                {
                    duration /= 2.0;
                }

                int integerDuration = (int)duration;

                string videoPath = videoFilePath;
                string thumbnailPath = Path.Combine(videoDirectory, "thumbnail.png");

                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $"-v quiet -i \"{videoPath}\" -ss {duration.ToString()} -vframes 1 \"{thumbnailPath}\"",
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();
            }
            catch(Exception exc)
            {
                Console.WriteLine($"[{hash}]:" + exc);
                _logger.LogInformation($"[{hash}]:" + exc);
            }
            
            Console.WriteLine($"[{hash}]: Thumbnail generation finished");
            _logger.LogInformation($"[{hash}]: Thumbnail generation finished");
        }
    }
}