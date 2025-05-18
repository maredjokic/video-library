using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Video_Library_Api.Models;
using Video_Library_Api.Repositories;
using Video_Library_Api.Services.Semaphores;

namespace Video_Library_Api.Services
{
    public abstract class BackgroundService
    {
        protected readonly IServiceScopeFactory _scopeFactory;
        protected readonly ILogger<BackgroundService> _logger;
        protected BackgroundServiceSemaphore _semaphore;
        
        public BackgroundService(
            IServiceScopeFactory scopeFactory, 
            ILogger<BackgroundService> logger,
            BackgroundServiceSemaphore semaphore)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _semaphore = semaphore;
        }
        
        public Task Run(Video video)
        {
            var backgroundTask = Task.Run(async () => 
            {
                try
                {
                    using(var scope = _scopeFactory.CreateScope())
                    {
                        var videoRepository = scope.ServiceProvider.GetService<IVideoRepository>();
                        var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();


                        await _semaphore.WaitAsync();
                        await StartAsync(video, scope);
                        // video object is disposed when trying to decrement number of processes left
                        var newVideo = await videoRepository.FindByIdAsync(video.Id);
                        newVideo.ProcessesLeft--;

                        await unitOfWork.CompleteAsync();
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            });
            return backgroundTask;
        }
        protected abstract Task StartAsync(Video video, IServiceScope scope);
    }
}