using Dragza.Application.Interface;
using Dragza.Infrastructure.Helper;
using Dragza.Infrastructure.Services;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;


namespace Dragza.Infrastructure.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly FirebaseApp _firebaseApp;
        private readonly JWTService _jWTService;

        public NotificationService( IConfiguration configuration, JWTService jWTService)
        {
           
            _jWTService = jWTService;
            var serviceAccountKeyPath = configuration["Firebase:serviceAccountKey"];
            //var serviceAccountKeyPath = configuration.;

            if (FirebaseApp.DefaultInstance == null)
            {
                _firebaseApp = FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(serviceAccountKeyPath)
                });
            }

        }

        public async Task<bool> SendNotificationAsync(string fireBaseId, string title, string body, object? data = null)
        {
            if (string.IsNullOrEmpty(fireBaseId))
                return false;
            Dictionary<string, string>? dataDic = null;
            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                dataDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            var message = new FirebaseAdmin.Messaging.Message
            {
                Token = fireBaseId,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = title,
                    Body = body
                },
                Data = dataDic
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return !string.IsNullOrEmpty(response);
            }
            catch (Exception ex)
            {
                // Log exception
                return false;
            }
        }
    }

}
