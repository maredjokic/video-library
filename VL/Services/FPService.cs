using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Video_Library_Api.Extensions;
using Video_Library_Api.Models;
using Video_Library_Api.Repositories;
using Video_Library_Api.Services.Semaphores;
using Video_Library_Api.Vendor.MotionDSP.Copyright;

namespace Video_Library_Api.Services
{
    public class FPService : BackgroundService
    {
        public FPService(
            IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundService> logger,
            IConfiguration configuration,
            FPServiceSemaphore semaphore)
            : base(scopeFactory, logger, semaphore)
        { }
        protected async override Task StartAsync(Video video, IServiceScope scope)
        {
            string hash = video.Id;
            string videoDirectory = video.GetDirectory();
            string videoName = video.FileName;
            string videoFilePath = video.GetVideoFilePath();
            
            Console.WriteLine($"[{hash}]: FP started");
            _logger.LogInformation($"[{hash}]: FP started");

            try
            {
                var fpUnitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                var videoRepository = scope.ServiceProvider.GetService<IVideoRepository>();
                var relatedVideosRepository = scope.ServiceProvider.GetService<IRelatedVideosRepository>();

                #if Linux
                var videoTask = videoRepository.FindByIdAsync(hash);
                string fpPath = Matcher.GenerateFp(videoFilePath, videoDirectory, videoName);
                byte[] fingerPrint = await File.ReadAllBytesAsync(fpPath);

                ulong videoID = ulong.Parse(hash, System.Globalization.NumberStyles.HexNumber);
                Matcher.Match(videoID, fingerPrint, (ushort) fingerPrint.Length, 0.5f);

                int n = Matcher.MatchesSize();
                for(int i = 0; i < n; i++)
                {
                    Match match = Matcher.GetMatch(i);
                    // random videoID1 fix
                    match.VideoID1 = videoID;
                    string video1 = match.VideoID1.ToString("x16");
                    string video2 = match.VideoID2.ToString("x16");
                    Console.WriteLine($"{video1} {video2} : {match.Score} length: {match.Length}");
                    if(video1 != video2)
                    {
                        relatedVideosRepository.Add(match);
                    }
                }

                Matcher.AddVideo(videoID, fingerPrint, (ushort) fingerPrint.Length);
                await videoTask;
                await fpUnitOfWork.CompleteAsync();
                #endif
            }
            catch(Exception exc)
            {
                Console.WriteLine($"[{hash}]: FP throw exception");
                Console.WriteLine($"[{hash}]: " + exc);
                _logger.LogInformation($"[{hash}]: throw exception");
                _logger.LogInformation($"[{hash}]: " + exc);
            }
            
            Console.WriteLine($"[{hash}]: FP finished");
            _logger.LogInformation($"[{hash}]: FP finished");
        }
    }
}