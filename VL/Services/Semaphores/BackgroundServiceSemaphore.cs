using System.Threading;
using System.Threading.Tasks;

namespace Video_Library_Api.Services.Semaphores
{
    public abstract class BackgroundServiceSemaphore
    {
        protected SemaphoreSlim _semaphore;

        public Task WaitAsync()
        {
            return _semaphore.WaitAsync();
        }

        public int Release()
        {
            return _semaphore.Release();
        }

    }
}