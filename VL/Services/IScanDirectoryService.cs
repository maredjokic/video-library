using System.Collections.Generic;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Resources;

namespace Video_Library_Api.Services
{
    public interface IScanDirectoryService
    {
        Task<string> ScanDirectory(string directoryPath, bool transcodeMP4, bool fpmatch);
        Task<List<DirectoryInf>> GetDirectories();
        Task<DirectoryInf> GetDirectoryInfo(string path);
        Task<DirectoryInf> GetDirectoryInfoByHash(string hash);
        Task<List<DirectoryEntry>> GetAllEntries();
        Task<List<DirectoryEntry>> GetEntries(string path);
        Task<List<DirectoryEntry>> GetEntriesByHash(string hash);
        Task<DirectoryEntry> UpdateDirectoryEntry(DirectoryEntry directoryEntry);
        Task<DirectoryInf> UpdateDirectoryInfo(DirectoryInf directoryInf);
        Task<DirectoryEntry> DeleteDirectoryEntryAsync(DirectoryEntry directoryEntry);
        Task<DirectoryInf>  DeleteDirectoryInfosAsync(DirectoryInf directoryInf);
        Task<string> PauseScanning(string path);
        Task<string> ResumeScanning(string path);
        Task<string> Cleanup(string path);
        Task<int> TotalEntryCount(string hash);
        Task<int> FinishedEntryCount(string hash);

    }
}