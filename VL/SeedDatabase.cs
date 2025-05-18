using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using Video_Library_Api.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Video_Library_Api.Models;
using Video_Library_Api.Repositories;
using Video_Library_Api.Contexts;

namespace Video_Library_Api
{
    public class SeedDatabase
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private static bool Pause;
        private static string DirectoryPath;
        

        public SeedDatabase(IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;

            Pause = false;
            DirectoryPath = "";
        }

        public string GetDirectoryPath()
        {
            return DirectoryPath;
        }

        public void SetPause(string path)
        {
            if(DirectoryPath == path)
            {
                Pause = true;
            }
            else
            {
                throw new Exception("This directory is not currently being processed!");
            }
        }

        //need changes
        public async Task AddVideos(DirectoryInf directory, string storagePath, bool transcodeMP4, bool fpmatch)//for creating  database
        {
            DirectoryPath = directory.Path;

            foreach (DirectoryEntry directoryEntry in directory.DirectoryEntries)
            {
                if(Pause)
                {
                    Console.WriteLine("Scanning paused on file: " + directoryEntry.FilePath);
                    break;
                }

                try
                {
                    if(directoryEntry.Status == "pending")
                    {
                        using(var scope = _scopeFactory.CreateScope())
                        {
                            directoryEntry.Status = "processing";
                            var scanDirService = scope.ServiceProvider.GetService<IScanDirectoryService>();
                            await scanDirService.UpdateDirectoryEntry(directoryEntry);

                            var videoService = scope.ServiceProvider.GetService<IVideoService>();
                            {
                                try
                                {
                                    Video video = await videoService.SaveAsync(directoryEntry.FilePath, Path.GetFileName(directoryEntry.FilePath), storagePath, transcodeMP4, fpmatch);
                                    
                                    directoryEntry.VideoId = video.Id;

                                    int ProcessesLeft = 1;
                                    do
                                    {
                                        Video v = await videoService.FindAsync(video.Id);

                                        try
                                        {
                                            videoService.ReloadEntity(v);
                                        }
                                        catch(Exception){}

                                        ProcessesLeft = v.ProcessesLeft;
                                        await Task.Delay(200);
                                    } 
                                    while(ProcessesLeft != 0);
                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine(exc);
                                    directoryEntry.Status = "failed";
                                    scanDirService = scope.ServiceProvider.GetService<IScanDirectoryService>();
                                    await scanDirService.UpdateDirectoryEntry(directoryEntry);
                                    Console.WriteLine("FAILED");
                                }
                                finally
                                {
                                    if(directoryEntry.Status != "failed")
                                    {
                                        directoryEntry.Status = "finished";
                                        scanDirService = scope.ServiceProvider.GetService<IScanDirectoryService>();
                                        await scanDirService.UpdateDirectoryEntry(directoryEntry);
                                        Console.WriteLine("FINISHED");
                                    }
                                }
                            }
                        }
                    }
                }
                catch(Exception )//exc)
                {
                    //Console.WriteLine(exc); proveriti exception u buducnosti
                }
            }

            using(var scope = _scopeFactory.CreateScope())
            {
                if(Pause)
                {
                    directory.Status = "paused";
                }
                else
                {
                    directory.Status = "finished";
                }

                var scanDirService = scope.ServiceProvider.GetService<IScanDirectoryService>();
                await scanDirService.UpdateDirectoryInfo(directory);
            }

            Pause = false;
            DirectoryPath = "";
        }
    }
}
