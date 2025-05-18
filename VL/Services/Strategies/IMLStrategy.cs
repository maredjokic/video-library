using System.Threading.Tasks;
using Video_Library_Api.Models;

namespace Video_Library_Api.Services.Strategies
{
    public interface IMLStrategy
    {
        Task<string> GetJsonAsync(Video video);
    }
}