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
    public class PreviewService : BackgroundService
    {
        public PreviewService(
            IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundService> logger,
            BackgroundCPUServiceSemaphore semaphore)
            : base(scopeFactory, logger, semaphore)
        { }
        
        protected async override Task StartAsync(Video video, IServiceScope scope)
        {
            string hash = video.Id;
            string videoDirectory = video.GetDirectory();
            string videoName = video.FileName;
            double? duration = video.Duration;
            string videoFilePath = video.GetVideoFilePath();
            
            Console.WriteLine($"[{hash}]: Preview generating started");
            _logger.LogInformation($"[{hash}]: Preview generating started");
            
            try
            {
                var videoService = scope.ServiceProvider.GetService<IVideoService>();

                var preview = Task.Run(() =>
                {
                    SavePreview(videoFilePath, videoDirectory, videoName, duration);
                });

                await preview;
            }
            catch(Exception exc)
            {
                Console.WriteLine($"[{hash}]:" + exc);
                _logger.LogInformation($"[{hash}]:" + exc);
            }

            Console.WriteLine($"[{hash}]: Preview generating finished");
            _logger.LogInformation($"[{hash}]: Preview generating finished");
        }

        private void SavePreview(string videoFilePath, string videoDirectory, string videoName, double? duration)
        {
            string videoPath = videoFilePath;
            string previewPath = Path.Combine(videoDirectory, "preview.png");

            if(!File.Exists(videoPath))
            {
                Console.WriteLine("Video not exist in storage.");
                return;
            }

            if(duration == 0.0)
                duration = null;

            double? speed;
            string arguments;
            if (duration != null)
            { 
                speed = 7 / duration; // 7sekundi / 1,25 frejma/sekundi ~~ 5-6 frejma

                arguments = $"-v quiet -i \"{videoPath}\" -r 1.25 -s 320x240 -filter_complex \"[0:v] setpts={speed.ToString()}*PTS\" -c:v apng -f apng {previewPath}\"";
            }//-filter:v   \"setpts={speed.ToString()}*PTS\"
            else
            {
                arguments = $"-v quiet -i \"{videoPath}\" -s 320x240 -ss 0 -vframes 1  -c:v apng -f apng \"{previewPath}\"";
            }

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

            if(!File.Exists(previewPath) || File.ReadAllText(previewPath)=="")//if -filter_complex isnt work
            {
                if (File.Exists(previewPath))
                {
                    File.Delete(previewPath);
                }
                if (duration != null)
                {   
                    speed = 7 / duration;
                    arguments = $"-v quiet -i \"{videoPath}\" -r 1.25 -s 320x240 -filter:v \"setpts={speed.ToString()}*PTS\" -c:v apng -f apng \"{previewPath}\"";
                }
                else
                {
                    arguments = $"-v quiet -i \"{videoPath}\" -s 320x240 -ss 0 -vframes 1  -c:v apng -f apng \"{previewPath}\"";
                }


                process = new Process()
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
            }
        }
    }
}