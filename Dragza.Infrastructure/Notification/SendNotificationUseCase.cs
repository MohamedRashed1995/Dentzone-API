using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Notification
{
    public class SendNotificationUseCase
    {
        private readonly INotificationService _notificationService;

        public SendNotificationUseCase(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<bool> ExecuteAsync(NotificationRequest request)
        {
            var fireBaseId = "";//await _clientDeviceRepository.GetFirebaseIdByDeviceKeyAsync(request.DeviceKey, request.ClientId);

            if (string.IsNullOrEmpty(fireBaseId))
                return false;

            var result = await _notificationService.SendNotificationAsync(fireBaseId, request.Title, request.Body, request.Data);
            //var systemNotification = new SystemsNotification
            //{
            //    ClientId = request.ClientId,
            //    Id = Guid.NewGuid(),
            //    NotificationDate = DateTime.Now,
            //    NotificationText = request.Body,
            //    NotificationTypeId = request.Type,
            //};
            //if (result == true)
            //    systemNotification.StatusId = (int)NotificationStatusEnum.deliverd;
            //else
            //    systemNotification.StatusId = (int)NotificationStatusEnum.undeliverd;

            //await _systemNotificationRepository.AddAsync(systemNotification);

            return result;
        }
    }

}
