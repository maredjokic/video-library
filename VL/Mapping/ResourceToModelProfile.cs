using AutoMapper;
using Video_Library_Api.Models;
using Video_Library_Api.Resources;

namespace Video_Library_Api.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<VideoResource, Video>();
            CreateMap<VideoSaveResource, Video>();
                
            CreateMap<TagResource, Tag>();
            CreateMap<TagSaveResource, Tag>();
        }
    }
}