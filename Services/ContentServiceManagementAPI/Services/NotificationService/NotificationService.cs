using ANQ.Notification.Services;
using ContentServiceManagementAPI.Constants;
using ContentServiceManagementAPI.Models.DTO.ContentMappingDto;
using ContentServiceManagementAPI.Models.DTO.ServiceMappingDto;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        IProcessNotification _processNotification;
        private readonly ILogger<NotificationService> logger;

        public NotificationService(
            IProcessNotification processNotification,
            ILogger<NotificationService> logger
            )
        {
            _processNotification = processNotification;
            this.logger = logger;
        }

        public async Task SendContentProvider_ContentServiceProviderMapNotification(AddMapContentDto model)
        {
            try
            {
                string NotificationTemplate = NotificationTemplates.MapNotification;
                NotificationTemplate = NotificationTemplate
                    .Replace("{user}", $"{model.ContentProviderName}")
                    .Replace("{message}", 
                    "The service provider <b>" + model.ServiceProviderName + "</b> has been mapped to <b>"
                    + model.ContentName + "</b> content."
                    );

                await Task.Run(() =>
                {
                    _processNotification.ProcessNotificationAsync(
                        "Client Management",
                        NotificationTemplate,
                        model.ContentProviderEmail
                    );
                });
            }
            catch(Exception e)
            {
                logger.LogError(e.Message, e);
            }
        }

        public async Task SendServiceProvider_ContentServiceProviderMapNotification(AddMapContentDto model)
        {
            try
            {
                string NotificationTemplate = NotificationTemplates.MapNotification;
                NotificationTemplate = NotificationTemplate
                    .Replace("{user}", $"{model.ServiceProviderName}")
                    .Replace("{message}",
                    "You have just been mapped to <b>" + model.ContentName + "</b> content."
                    );

                await Task.Run(() =>
                {
                    _processNotification.ProcessNotificationAsync(
                        "Client Management",
                        NotificationTemplate,
                        model.ServiceProviderEmail
                    );
                });
            }
            catch(Exception e)
            {
                logger.LogError(e.Message, e);
            }
        }


        public async Task SendClient_ServiceClientMapNotification(AddMapServiceDto model)
        {
            try
            {
                string NotificationTemplate = NotificationTemplates.MapNotification;
                NotificationTemplate = NotificationTemplate
                    .Replace("{user}", $"{model.ClientName}")
                    .Replace("{message}",
                    "You have just been mapped to <b>" + model.ServiceName + "</b> service."
                    );

                await Task.Run(() =>
                {
                    _processNotification.ProcessNotificationAsync(
                        "Client Management",
                        NotificationTemplate,
                        model.ClientEmailAddress
                    );
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
            }
        }

        public async Task SendServiceProvider_ServiceClientMapNotification(AddMapServiceDto model)
        {
            try
            {
                string NotificationTemplate = NotificationTemplates.MapNotification;
                NotificationTemplate = NotificationTemplate
                    .Replace("{user}", $"{model.ServiceProviderName}")
                    .Replace("{message}",
                    "The client <b>" + model.ClientName + "</b> has just been mapped to <b>" + model.ServiceName + "</b> service."
                    );

                await Task.Run(() =>
                {
                    _processNotification.ProcessNotificationAsync(
                        "Client Management",
                        NotificationTemplate,
                        model.ServiceProviderEmail
                    );
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
            }
        }
    }
}
