using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Notification
{
    public class SendNotificationBackgroundService : BackgroundService
    {
        private readonly NotificationQueue _notificationQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SendNotificationBackgroundService> _logger;

        public SendNotificationBackgroundService(NotificationQueue notificationQueue, IServiceScopeFactory serviceScopeFactory, ILogger<SendNotificationBackgroundService> logger)
        {
            _notificationQueue = notificationQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Notification Background Service is running.");
            _logger.LogInformation("Notification Background Service is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var request = await _notificationQueue.DequeueAsync(stoppingToken);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var useCase = scope.ServiceProvider.GetRequiredService<SendNotificationUseCase>();

                    try
                    {
                        var result = await useCase.ExecuteAsync(request);
                        Console.WriteLine($"Notification processed. Success: {result}");
                        _logger.LogInformation($"Notification processed for {request.ClientId}. Success: {result}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation($"Error processing notification for {request.ClientId}: {ex.Message}");
                        // Log or handle error
                    }
                }
            }

            _logger.LogInformation("Notification Background Service is stopping.");
        }
    }

}
