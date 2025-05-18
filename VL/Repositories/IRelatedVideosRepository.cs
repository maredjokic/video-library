using System.Collections.Generic;
using System.Threading.Tasks;
using Video_Library_Api.Models;
using Video_Library_Api.Vendor.MotionDSP.Copyright;

namespace Video_Library_Api.Repositories
{
    public interface IRelatedVideosRepository
    {
        RelatedVideos Add(RelatedVideos relatedVideos);
        #if Linux
        RelatedVideos Add(Match match);
        #endif
        Task<IEnumerable<RelatedVideos>> ListRelatedAsync(RelatedVideos relatedVideos);
        RelatedVideos Update(RelatedVideos relatedVideos);
        RelatedVideos Remove(RelatedVideos relatedVideos);
        Task<IEnumerable<RelatedVideos>> ListAsync();
    }
}