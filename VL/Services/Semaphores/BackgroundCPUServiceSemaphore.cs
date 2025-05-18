using System.Threading;
using Microsoft.Extensions.Configuration;

namespace Video_Library_Api.Services.Semaphores
{
    public class BackgroundCPUServiceSemaphore : BackgroundServiceSemaphore
    {
        public BackgroundCPUServiceSemaphore(IConfiguration configuration)
        {
            int maxQueueLength = configuration.GetValue<int>("CPUTaskQueueSize");
            _semaphore = new SemaphoreSlim(maxQueueLength);
        }
    }
}