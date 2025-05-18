using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Video_Library_Api.Extensions;
using Video_Library_Api.Models;

namespace Video_Library_Api.Services.Strategies
{
    public class MLProductionStrategy : IMLStrategy
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<MLProductionStrategy> _logger;

        public MLProductionStrategy(IHttpClientFactory clientFactory, ILogger<MLProductionStrategy> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<string> GetJsonAsync(Video video)
        {
            var client = _clientFactory.CreateClient("ml");

            var parameters = new Dictionary<string, string> 
            { 
                { "VideoPath", "/wwwroot/" + video.GetRelativePath() },
                { "VideoId", video.Id } 
            };
            _logger.LogInformation(parameters["VideoPath"]);
            var encodedContent = new FormUrlEncodedContent(parameters);

            var response = await client.PostAsync("/", encodedContent);

            return await response.Content.ReadAsStringAsync();
        }
    }
}