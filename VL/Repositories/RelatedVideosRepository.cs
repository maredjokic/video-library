using System.Collections.Generic;
using System.Threading.Tasks;
using Video_Library_Api.Contexts;
using Video_Library_Api.Models;
using Video_Library_Api.Vendor.MotionDSP.Copyright;

namespace Video_Library_Api.Repositories
{
    public class RelatedVideosRepository : BaseRepository, IRelatedVideosRepository
    {
        public RelatedVideosRepository(AppDbContext context) : base(context) {}
        public RelatedVideos Add(RelatedVideos relatedVideos)
        {
            _context.RelatedVideos.Add(relatedVideos);
            return relatedVideos;
        }

        #if Linux
        public RelatedVideos Add(Match match)
        {
            RelatedVideos rv = new RelatedVideos();
            rv.Video1Id = match.VideoID1.ToString("x16");
            rv.Video2Id = match.VideoID2.ToString("x16");
            rv.Offset1 = match.Offset1;
            rv.Offset2 = match.Offset2;
            rv.Score = match.Score;

            return Add(rv);
        }
        #endif

        public Task<IEnumerable<RelatedVideos>> ListAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<RelatedVideos>> ListRelatedAsync(RelatedVideos relatedVideos)
        {
            throw new System.NotImplementedException();
        }

        public RelatedVideos Remove(RelatedVideos relatedVideos)
        {
            throw new System.NotImplementedException();
        }

        public RelatedVideos Update(RelatedVideos relatedVideos)
        {
            throw new System.NotImplementedException();
        }
    }
}