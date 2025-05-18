using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Video_Library_Api.Models;
using Video_Library_Api.Resources;
using Video_Library_Api.StreamsData;
using Video_Library_Api.Exceptions;
using Video_Library_Api.Extensions;




namespace Video_Library_Api.Services
{
    public class FeedsService : IFeedsService
    {
        protected readonly IServiceScopeFactory _scopeFactory;
        protected readonly ILogger<BackgroundService> _logger;
        protected readonly IVideoService _videoService;

        private readonly IHostingEnvironment _hostingEnvironment;        
        private readonly StreamsDictionary _streamsDictionary;

        public FeedsService(IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundService> logger,
            IVideoService videoService,
            IHostingEnvironment hostingEnvironment,
            StreamsDictionary streamsDictionary)
        {
                _scopeFactory = scopeFactory;
                _logger = logger;
                _videoService = videoService;
                _hostingEnvironment = hostingEnvironment;
                _streamsDictionary = streamsDictionary;
        }
        
        public IEnumerable<FeedProcessResource> GetAll()
        {
            return _streamsDictionary.mdspprocProcesses.Values.ToList();
        }

        public FeedProcessResource GetById(int id)
        {
            return _streamsDictionary.mdspprocProcesses[id];
        }

        public async Task<FeedResource> StartStream(FeedResourceInput feedResource)
        {
            if(!CheckURL(feedResource.URL))
            {
                throw new NotFoundException("Udp address is not valid!");
            }

            foreach(var item in _streamsDictionary.mdspprocProcesses)
            {
                if(item.Value.FeedResource.URL.Equals(feedResource.URL))
                {
                    throw new NotFoundException("URL address is already taken!");
                }
            }

            Video video = await _videoService.FindAsync(feedResource.VideoId);

            string udp_ip = feedResource.URL;
            string iniFilePath = "/home/marko/Desktop/inifile.ini";//PROMENITI!
            string videoPath = video.GetVideoFilePath();

            string arguments;

            if(feedResource.Loop == true)
            {
                arguments = "-i " + "\"" + videoPath  +  "\""  + " -o " + udp_ip + " -c " + iniFilePath + " -loop -transcode-only";
            }
            else
            {
                arguments = "-i " + "\""  + videoPath  + "\""  + " -o " + udp_ip + " -c " + iniFilePath + " -transcode-only";
            }

            
            Console.WriteLine(arguments);

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "mdspproc", 
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Exited += null;
            process.EnableRaisingEvents = true;
            
            process.Exited += processExited_handler;

            process.Start();
            
            FeedResource thisFeedResource = new FeedResource();

            thisFeedResource.VideoId = feedResource.VideoId;
            thisFeedResource.URL = feedResource.URL;
            thisFeedResource.Loop = feedResource.Loop;
            thisFeedResource.Active = true;
            thisFeedResource.FeedId = process.Id;

            FeedProcessResource feedProcessResource = new FeedProcessResource();
            feedProcessResource.FeedResource = thisFeedResource;
            feedProcessResource.Process = process;

            _streamsDictionary.mdspprocProcesses.Add(process.Id, feedProcessResource);

            return thisFeedResource;
        }

        private void processExited_handler(object sender, EventArgs args)
        {
            Process p = sender as Process;
            if (p != null && p.HasExited)
            {
                foreach(FeedProcessResource fpr in _streamsDictionary.mdspprocProcesses.Values)
                {
                    if(fpr.Process.Id == p.Id)
                    {
                        fpr.FeedResource.Active = false;
                        Console.WriteLine("The stream process with the id " + fpr.FeedResource.FeedId  + " was exited!");
                    }
                }
            }
        }

        public bool StopStream(int id)
        {
            if(_streamsDictionary.mdspprocProcesses.ContainsKey(id))
            {
                FeedProcessResource feedProcessResource = _streamsDictionary.mdspprocProcesses[id];
                _streamsDictionary.mdspprocProcesses.Remove(id);

                if(!feedProcessResource.Process.HasExited)
                {
                    feedProcessResource.Process.Kill(); 
                    feedProcessResource.Process.Dispose();
                }

                Console.WriteLine("The stream process with the id " + feedProcessResource.FeedResource.FeedId  + " was stopped and disposed!");
                
                return true;
            }   
            else
            {
                throw new NotFoundException("The stream process with id "+ id + " doesn't exists!");
            }
        }

        public FeedResource StartStreamAgain(int feedId)
        {
            if(_streamsDictionary.mdspprocProcesses.ContainsKey(feedId))
            {
                FeedProcessResource feedProcessResource = _streamsDictionary.mdspprocProcesses[feedId];

                if(feedProcessResource.FeedResource.Loop == false && feedProcessResource.FeedResource.Active == false)
                {
                    feedProcessResource.Process.Start();
                    feedProcessResource.FeedResource.Active = true;

                    return feedProcessResource.FeedResource;
                }
                else
                {
                    throw new AlreadyActiveStreamException();
                }
            }
            else
            {
                throw new NotFoundException("The stream process with id "+ feedId + " doesn't exists!");
            }

        }

        private bool CheckURL(string url)
        {
            string thisUrl = url;
            string toBeSearched = "udp://";
            string addressPort = thisUrl.Substring(thisUrl.IndexOf(toBeSearched) + toBeSearched.Length);
            int index = addressPort.IndexOf(":");
            string address;

            if (index > 0)
                address = addressPort.Substring(0, index);
            else return false;

            if(!ValidateIPv4(address))
                return false;

            string port = addressPort.Substring(addressPort.IndexOf(":") + 1);
            if( int.Parse(port) < 0 || int.Parse(port) > 65535)
            {
                return false;
            }

            return true;
        }

        private bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            byte tempForParsing;

            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

    }
}