using System.Threading;

namespace Video_Library_Api.Services.Semaphores
{
    public class FPServiceSemaphore : BackgroundServiceSemaphore
    {
        public FPServiceSemaphore()
        {
            _semaphore = new SemaphoreSlim(1);
        }
    }
}