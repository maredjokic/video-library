using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Video_Library_Api.Extensions;
using Video_Library_Api.Models;
using Video_Library_Api.Repositories;
using Video_Library_Api.Services.Semaphores;

namespace Video_Library_Api.Services
{
    public class TranscodingService : BackgroundService
    {
        public TranscodingService(
            IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundService> logger,
            BackgroundCPUServiceSemaphore semaphore)
            : base(scopeFactory, logger, semaphore)
        { }
        protected override Task StartAsync(Video video, IServiceScope scope)
        {
            string hash = video.Id;
            string videoDirectory = video.GetDirectory();
            string videoName = video.FileName;
            string videoFilePath = video.GetVideoFilePath();

            return Task.Run(() =>
            {
                Transcoding(videoFilePath, hash, videoDirectory, videoName);
            });
        }
        public void Transcoding(string videoFilePath, string hash, string videoDirectory, string videoName)
        {
            string videoPath = videoFilePath;
            string rawVideoName = Path.GetFileNameWithoutExtension(videoName);
            string videoTranscodedPath = Path.Combine(videoDirectory, rawVideoName + "_transcoded.mp4");
            string arguments = $"-v quiet -i \"{videoPath}\" -s hd480 -codec:v h264 -profile:v high -codec:a aac -preset slow \"{videoTranscodedPath}\"";

            try
            {
                Console.WriteLine($"[{hash}]: Transcoding started");
                _logger.LogInformation($"[{hash}]: Transcoding started");

                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                Console.WriteLine($"[{hash}]: Transcoding finished");
                _logger.LogInformation($"[{hash}]: Transcoding finished");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}