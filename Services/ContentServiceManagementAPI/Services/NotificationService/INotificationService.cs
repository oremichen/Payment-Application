using ContentServiceManagementAPI.Models.DTO.ContentMappingDto;
using ContentServiceManagementAPI.Models.DTO.ServiceMappingDto;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.NotificationService
{
    public interface INotificationService
    {
        Task SendContentProvider_ContentServiceProviderMapNotification(AddMapContentDto model);
        Task SendServiceProvider_ContentServiceProviderMapNotification(AddMapContentDto model);



        Task SendServiceProvider_ServiceClientMapNotification(AddMapServiceDto model);
        Task SendClient_ServiceClientMapNotification(AddMapServiceDto model);
    }
}
