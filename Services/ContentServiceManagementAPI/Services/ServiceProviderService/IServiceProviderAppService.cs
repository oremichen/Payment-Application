using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DTO;
using ContentServiceManagementAPI.Models.DTO.ServiceProvider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.ServiceProviderService
{
    public interface IServiceProviderAppService
    {
        Task<ServiceProvider> AddServiceProvider(ServiceProvider model);

        Task<IEnumerable<ServiceProviderDto>> GetServiceProviders();

        Task UpdateApprovedStatus(ServiceProviderDto serviceProviderDto);

        Task<ServiceProviderDto> GetServiceProvider(long serviceProviderId);

        Task<ServiceProviderDto> GetServiceProviderByAuthId(long authId);

        Task UpdateDndServiceProvider(DndSpUpdateRequest dndSpUpdateRequest);
    }
}