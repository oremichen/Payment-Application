using ContentServiceManagementAPI.Models.DTO.Client;
using ContentServiceManagementAPI.Models.DTO.ContentProvider;
using ContentServiceManagementAPI.Models.DTO.ServiceProvider;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.TenantUpdateService
{
    public interface IUpdateTenantsService
    {
        Task UpdateClient(UserProfileClientDto model);

        Task UpdateServiceProvider(UserProfileServiceProviderDto model);

        Task UpdateContentProvider(UserProfileContentProviderDto model);
    }
}
