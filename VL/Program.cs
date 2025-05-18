using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Video_Library_Api.Vendor.MotionDSP.Copyright;

namespace Video_Library_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // NLog: setup the logger first to catch all errors
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                var host = CreateWebHostBuilder(args).Build();

                var hostingEnvironment = (IHostingEnvironment) host.Services.GetService(typeof(IHostingEnvironment));

                Environment.SetEnvironmentVariable(
                    "WebRootPath", hostingEnvironment.WebRootPath);

                InitializeStorage(hostingEnvironment);
                InitializeFpMatch(hostingEnvironment);

                host.Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();  // NLog: setup NLog for Dependency injection
        
        private static void InitializeStorage(IHostingEnvironment hostingEnvironment)
        {
            // Create storage folder
            var storagePath = Path.Combine(hostingEnvironment.WebRootPath, "storage");
            var scanDirecrotiesPath = Path.Combine( storagePath, "scan");
            
            if(!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }

            if(!Directory.Exists(scanDirecrotiesPath))
            {
                Directory.CreateDirectory(scanDirecrotiesPath);
            }
        }
        private static void InitializeFpMatch(IHostingEnvironment hostingEnvironment)
        {
            var storagePath = Path.Combine(hostingEnvironment.WebRootPath, "storage");
            //TO DOOOOOOOO


            // Add all fingerprints to Fpmatch
            string[] filePaths = Directory.GetFiles(storagePath, "*.fp", SearchOption.AllDirectories);

            foreach (string file in filePaths)
            {
                string dir = Path.GetDirectoryName(file);
                string hash = dir.Substring(dir.Length - 17).Replace("/", "").Replace(@"\", "");

                byte[] fp = File.ReadAllBytes(file); // only when restarting server, no need to be async
                
                ulong videoID = ulong.Parse(hash, System.Globalization.NumberStyles.HexNumber);

                #if Linux
                try
                {
                    Matcher.AddVideo(videoID, fp, (ushort) fp.Length);
                }
                catch(Exception exc)
                {
                    Console.WriteLine(exc);
                }
                #endif
            }
        }
    }
}
