using AutoMapper;
using Microsoft.AspNetCore.Http;
using Video_Library_Api.Models;
using Video_Library_Api.Resources;

namespace Video_Library_Api.Mapping.Resolvers
{
    public class UrlResolver : IValueResolver<Video, VideoResource, string>
    {
        private readonly HttpRequest request;
        public UrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            request = httpContextAccessor.HttpContext.Request;
        }
        public string Resolve(Video source, VideoResource destination, string destMember, ResolutionContext context)
        {
            string relativePath = source.Id.Substring(0, 2) + "/" + source.Id.Substring(2, 14);
            if(source.StoragePath == null)
            {
                return $"{request.Scheme}://{request.Host}/storage/{relativePath}/";
            }
            else
            {
                return $"{request.Scheme}://{request.Host}/storage/scan/{source.StoragePath}/{relativePath}/";
            }
        }
    }
}