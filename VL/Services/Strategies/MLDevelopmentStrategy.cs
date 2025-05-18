using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Video_Library_Api.Extensions;
using Video_Library_Api.Models;

namespace Video_Library_Api.Services.Strategies
{
    public class MLDevelopmentStrategy : IMLStrategy
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public MLDevelopmentStrategy(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<string> GetJsonAsync(Video video)
        {
            string scriptPath = Path.Combine(
                _hostingEnvironment.ContentRootPath, 
                "pytorch-yolo",
                "detect_cmd.py");
            
            string fileName = RuntimeInformation
                .IsOSPlatform(OSPlatform.Windows) ? "python" : "/usr/bin/python3";

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = $"\"{scriptPath}\" --video \"{video.GetPath()}\" --videoid {video.Id}",
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string result = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            return result;
        }
    }
}