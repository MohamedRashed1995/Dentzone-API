using Dragza.Domain.DTO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Notification
{
    public class NotificationQueue
    {
        private readonly ConcurrentQueue<NotificationRequest> _queue = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void Enqueue(NotificationRequest request)
        {
            _queue.Enqueue(request);
            _signal.Release();
        }

        public async Task<NotificationRequest> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _queue.TryDequeue(out var request);
            return request;
        }
    }
}
