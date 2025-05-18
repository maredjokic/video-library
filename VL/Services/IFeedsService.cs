using System.Collections.Generic;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Paging;
using Video_Library_Api.Resources;

namespace Video_Library_Api.Services
{
    public interface IFeedsService
    {
        Task<FeedResource> StartStream(FeedResourceInput feedResource);
        bool StopStream(int id);
        IEnumerable<FeedProcessResource> GetAll();
        FeedProcessResource GetById(int id);
        FeedResource StartStreamAgain(int feedId);
    }
}