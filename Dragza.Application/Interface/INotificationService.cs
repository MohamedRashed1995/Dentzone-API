using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface INotificationService
    {
        Task<bool> SendNotificationAsync(string fireBaseId, string title, string body, object? data = null);
    }
}
