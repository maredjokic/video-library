using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Video_Library_Api.Exceptions;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Repositories;
using Video_Library_Api.Vendor.SipHash;
using Video_Library_Api.Resources;
using Video_Library_Api.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Video_Library_Api.Services
{
    public class ScanDirectoryService : IScanDirectoryService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly SeedDatabase _seedDatabase;
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVideoService _videoService;
        public ScanDirectoryService(
            IConfiguration configuration,
            IHostingEnvironment hostingEnvironment,
            SeedDatabase seedDatabase,
            AppDbContext context,
            IUnitOfWork unitOfWork,
            IVideoService videoService)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _videoService = videoService;
            _seedDatabase = seedDatabase;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> ScanDirectory(string directoryPath, bool transcodeMP4, bool fpmatch)
        {
            if(_seedDatabase.GetDirectoryPath() != "")
            {
                throw new Exception("Certain directory is already being processing!");
            }

            string hash = GetHashFromPath(directoryPath);

            string storagePath = Path.Combine(
                _hostingEnvironment.WebRootPath,
                "storage",
                "scan",
                hash
            );

            if(!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }
            else
            {
                throw new Exception("Directory already added!");
            }

            Console.WriteLine("Dir path in storage:[ " + storagePath + " ].");

            string metadataDir = Path.Combine(
                storagePath,
                "metadata_dir"
            );

            if(!Directory.Exists(metadataDir))
            {
                Directory.CreateDirectory(metadataDir);
            }

            string infoPath = Path.Combine(
                storagePath,
                "metadata_dir",
                "info.txt"
            );

            if (!File.Exists(infoPath)) 
            {
                using (StreamWriter sw = File.CreateText(infoPath)) 
                {
                    sw.WriteLine(directoryPath);
                }
            }

            string[] filePaths = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
            
            DirectoryInf thisDirectory = new DirectoryInf(){
                DirectoryHash = hash,
                Status = "processing",
                Path = directoryPath
            };
            Console.WriteLine(thisDirectory);

            await _context.DirectoryInfos.AddAsync(thisDirectory);
            await _unitOfWork.CompleteAsync();

            foreach(string file in filePaths)
            {
                if(IsMediaFile(file))
                {
                    await _context.DirectoryEntries.AddAsync(new DirectoryEntry(){
                        FilePath = file,
                        DirectoryHash = hash,
                        Status = "pending",
                        VideoId = null,
                        DirectoryInf = thisDirectory
                    });
                    Console.WriteLine(file);
                }
            }

            await _unitOfWork.CompleteAsync();

            Task.Run( () => _seedDatabase.AddVideos(thisDirectory, hash, transcodeMP4, fpmatch));
            
            return hash;
        }

        public async Task<List<DirectoryInf>> GetDirectories()
        {
            return await _context.DirectoryInfos.ToListAsync();
        }

        public async Task<DirectoryInf> GetDirectoryInfo(string path)
        {
            string hash = GetHashFromPath(path);

            return await _context.DirectoryInfos.FindAsync(hash);
        }

        public async Task<DirectoryInf> GetDirectoryInfoByHash(string hash)
        {
            return await _context.DirectoryInfos.FindAsync(hash);
        }

        public async Task<DirectoryEntry> UpdateDirectoryEntry(DirectoryEntry directoryEntry)
        {
            _context.DirectoryEntries.Update(directoryEntry);
            await _unitOfWork.CompleteAsync();    
            return directoryEntry;
        }
        public async Task<DirectoryInf> UpdateDirectoryInfo(DirectoryInf directoryInf)
        {
            _context.DirectoryInfos.Update(directoryInf);
            await _unitOfWork.CompleteAsync();    
            return directoryInf;
        }


        public async Task<List<DirectoryEntry>> GetAllEntries()
        {
            return await _context.DirectoryEntries.ToListAsync();
        }

        public async Task<List<DirectoryEntry>> GetEntries(string path)
        {
            string hash = GetHashFromPath(path);

            return await _context.DirectoryEntries
                .Where(e=> e.DirectoryHash == hash)
                .ToListAsync();
        }

        public async Task<List<DirectoryEntry>> GetEntriesByHash(string hash)
        {
            return await _context.DirectoryEntries
                .Where(e=> e.DirectoryHash == hash)
                .ToListAsync();
        }

        public async Task<DirectoryEntry> DeleteDirectoryEntryAsync(DirectoryEntry directoryEntry)
        {
            _context.DirectoryEntries.Remove(directoryEntry);
            await _unitOfWork.CompleteAsync();  

            return directoryEntry;
        }

        public async Task<DirectoryInf>  DeleteDirectoryInfosAsync(DirectoryInf directoryInf)
        {
            string videoDirectoryPath = Path.Combine(
                    _hostingEnvironment.WebRootPath,
                    "storage",
                    "scan",
                    directoryInf.DirectoryHash
            );

            if(Directory.Exists(videoDirectoryPath))
                Directory.Delete(videoDirectoryPath, true);

            _context.DirectoryInfos.Remove(directoryInf);
            await _unitOfWork.CompleteAsync();  

            return directoryInf;
        }

        public async Task<string> PauseScanning(string path)
        {
            string hash = GetHashFromPath(path);
            DirectoryInf di = _context.DirectoryInfos.Find(hash);

            if(di.Status == "processing")
            {
                try
                {
                    _seedDatabase.SetPause(path);
                    di.Status = "paused";
                    _context.DirectoryInfos.Update(di);
                    await _unitOfWork.CompleteAsync();
                }
                catch(Exception){}
            }
            else
            {
                throw new Exception("Cannot pause because the directory is not processing!");
            }

            return hash;
        }

        public async Task<string> ResumeScanning(string path)
        {
            Console.WriteLine("dfsfd"+ _seedDatabase.GetDirectoryPath().ToString());
            if(_seedDatabase.GetDirectoryPath() != "")
            {
                throw new Exception("Certain directory is already being processing!");
            }

            string hash = GetHashFromPath(path);

            DirectoryInf directoryInfo = await _context.DirectoryInfos
                .Include(di => di.DirectoryEntries)
                .FirstOrDefaultAsync(di => di.Path == path);

            if(directoryInfo.Status != "paused")
            {
                throw new Exception("Processing of this directory is not paused!");
            }

            Console.WriteLine(directoryInfo.DirectoryEntries.Count);

            try
            {
                directoryInfo.Status = "processing";

                await _context.DirectoryInfos.AddAsync(directoryInfo);
                await _unitOfWork.CompleteAsync();
            }
            catch(Exception)
            {

            }

            Task.Run( async () => await _seedDatabase.AddVideos(directoryInfo, hash, true, false));

            return hash;
        }

        public async Task<string> Cleanup(string path)
        {
            if(_seedDatabase.GetDirectoryPath() != "")
            {
                throw new Exception("Certain directory is already being processing!");
            }

            string hash = GetHashFromPath(path);
            DirectoryInf directoryInfo = await _context.DirectoryInfos
                .Include(di => di.DirectoryEntries)
                .FirstOrDefaultAsync(di => di.Path == path);

            try
            {
                directoryInfo.Status = "processing";

                await _context.DirectoryInfos.AddAsync(directoryInfo);
                await _unitOfWork.CompleteAsync();
            }
            catch(Exception){}

            string[] files = Directory.GetFiles(directoryInfo.Path, "*", SearchOption.AllDirectories);

            List<string> videoFiles = new List<string>();
            foreach(string file in files)
            {
                if(IsMediaFile(file))
                {
                    videoFiles.Add(file);
                }
            }
            
            List<string> videoFilesFinished = new List<string>();
            foreach(DirectoryEntry de in directoryInfo.DirectoryEntries)
            {
                videoFilesFinished.Add(de.FilePath);
            }

            List<string> DeletedVideoFiles = videoFilesFinished.Except(videoFiles).ToList();

            foreach(string deletedVideoFile in DeletedVideoFiles)
            {
                Console.WriteLine(deletedVideoFile);
                DirectoryEntry entry = _context.DirectoryEntries.Find(deletedVideoFile);
                
                Console.WriteLine(entry.VideoId + entry.FilePath+ deletedVideoFile);

                await _videoService.DeleteAsync(entry.VideoId);
                _context.DirectoryEntries.Remove(entry);
                await _unitOfWork.CompleteAsync();
            }
            

            List<string> newVideoFiles = videoFiles.Except(videoFilesFinished).ToList();
            
            foreach(string newVideo in newVideoFiles)
            {
                await _context.DirectoryEntries.AddAsync(new DirectoryEntry(){
                            FilePath = newVideo,
                            DirectoryHash = hash,
                            Status = "pending",
                            VideoId = null,
                            DirectoryInf = directoryInfo
                        });

                await _unitOfWork.CompleteAsync();
            }

            Task.Run( () => _seedDatabase.AddVideos(directoryInfo, hash, true, false));

            return hash;
        }

        public string GetHashFromPath(string path)
        {
            string key = _configuration.GetValue<string>("SipHashKey");
            SipHash sipHash = new SipHash(Encoding.ASCII.GetBytes(key));

            byte[] bytes = Encoding.ASCII.GetBytes(path);
            string hash = sipHash.Compute(bytes).ToString("x16");

            return hash;
        }

        public async Task<int> TotalEntryCount(string hash)
        {
            return await _context.DirectoryEntries
                .Where(e => e.DirectoryHash == hash)
                .CountAsync();
        }

        public async Task<int> FinishedEntryCount(string hash)
        {
            return await _context.DirectoryEntries
                .Where(e => e.DirectoryHash == hash && e.Status == "finished")
                .CountAsync();

        }
        
        private bool IsMediaFile(string path)
        {
            string[] mediaExtensions = {
              ".WEBM", ".MKV", ".FLV", ".VOB",".OGV", ".OGG", ".DRC", ".GIF",".GIFV",
               ".MNG", ".AVI", ".MTS",".M2TS",".TS", ".MOV", ".QT", ".WMV", ".YUV", ".RM",
               ".RMVB", ".ASF", ".AMV", ".MP4" , ".M4P", ".M4V", ".MPG", ".MP2", ".MPEG",
                ".MPE", ".MPV", ".M2T", ".MPG", ".MPEG", ".M2V", ".M4V", ".SVI", ".3GP",
                ".3G2", ".MXF", ".ROQ", ".NSV", ".FLV", ".F4V", ".F4P", ".F4A", ".F4B"
            };

            return -1 != Array.IndexOf(mediaExtensions, Path.GetExtension(path).ToUpperInvariant());
        }
    }
}