using System;
using System.IO;
using System.Linq;
using System.Net;
using AutoMapper;
using Video_Library_Api.Mapping.Resolvers;
using Video_Library_Api.Models;
using Video_Library_Api.Resources;
using Video_Library_Api.Extensions;

namespace Video_Library_Api.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            
            CreateMap<Video, VideoResource>()
                .ForMember(dest => dest.Video, opt => opt.MapFrom<UrlResolver>())
                .ForMember(dest => dest.TranscodedVideo, opt => opt.MapFrom<UrlResolver>())
                .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom<UrlResolver>())
                .ForMember(dest => dest.Preview, opt => opt.MapFrom<UrlResolver>())
                .ForMember(dest => dest.FFProbe, opt => opt.MapFrom<UrlResolver>())
                .ForMember(dest => dest.KLV2JSON, opt => opt.MapFrom<UrlResolver>())
                .AfterMap((src, dest) => 
                {
                    if(dest.StoragePath == null)
                    {
                        dest.Video = Uri.EscapeUriString(src.FileName);
                    }
                    else
                    {
                        dest.Video = src.GetVideoFilePath();
                    }
                    
                    dest.TranscodedVideo += Uri.EscapeUriString(
                        Path.GetFileNameWithoutExtension(src.FileName) + "_transcoded.mp4");
                    dest.Thumbnail += "thumbnail.png";
                    dest.Preview += "preview.png";
                    dest.FFProbe += "ffprobe.json";
                    dest.KLV2JSON += "klv2json.json";
                });

            CreateMap<Tag, TagResource>();
            CreateMap<RelatedVideos, RelatedVideosResource>();
            CreateMap<VideoGeolocation, VideoGeolocationResource>();
            CreateMap<DirectoryEntry,DirectoryEntryResource>();
            CreateMap<DirectoryInf,DirectoryInfResource>();
            CreateMap<VideosTags, VideosTagsResource>()
                .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.Tag.Name));
            CreateMap<VideosTags, VideosTagsDeleteResource>()
                .ForMember(dest => dest.TagName, opt => opt.MapFrom(src => src.Tag.Name));
        }
    }
}