using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Video_Library_Api.Services.Semaphores
{
    public class BackgroundGPUServiceSemaphore : BackgroundServiceSemaphore
    {
        public BackgroundGPUServiceSemaphore(IConfiguration configuration)
        {
            int maxQueueLength = configuration.GetValue<int>("GPUTaskQueueSize");
            _semaphore = new SemaphoreSlim(maxQueueLength);
        }
    }
}