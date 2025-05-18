using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Video_Library_Api.Models;

namespace Video_Library_Api.Extensions
{
    public static class VideoExtensions
    {
        public static string GetDirectory(this Video video)
        {
            if(video.StoragePath == null)
            {
                return Path.Combine(
                    Environment.GetEnvironmentVariable("WebRootPath"),
                    "storage", 
                    video.Id.Substring(0, 2),
                    video.Id.Substring(2, video.Id.Length - 2)
                );
            }
            else
            {
                string filePath = Path.Combine(
                    Environment.GetEnvironmentVariable("WebRootPath"),
                    "storage",
                    "scan",
                    video.StoragePath,
                    video.Id.Substring(0, 2),
                    video.Id.Substring(2, video.Id.Length - 2)
                );

                return filePath;
            }
        }

        public static string GetVideoFilePath(this Video video)
        {
            if(video.StoragePath == null)
            {
                return Path.Combine(GetDirectory(video),video.FileName);
            }
            else
            {
                string filePath = Path.Combine(
                    Environment.GetEnvironmentVariable("WebRootPath"),
                    "storage",
                    "scan",
                    video.StoragePath,
                    video.Id.Substring(0, 2),
                    video.Id.Substring(2, video.Id.Length - 2),
                    "videoPath.txt"
                );

                string videoFile;

                using(StreamReader sr = new StreamReader(filePath))
                {
                    videoFile = sr.ReadLine();
                }

                return videoFile;
            }
        }

        public static string GetPath(this Video video)
        {
            return Path.Combine(video.GetDirectory(), video.FileName);
        }

        public static string GetKlvPath(this Video video)
        {
            return Path.Combine(video.GetDirectory(), "klv2json.json");
        }

        public static string GetRelativePath(this Video video)
        {
            return Path.GetRelativePath(
                Environment.GetEnvironmentVariable("WebRootPath"),
                video.GetPath()
            );
        }
    }
}