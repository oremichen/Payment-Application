using ContentServiceManagementAPI.Models.DTO.DeliveryService;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.VasProviderAppService
{
    public interface IVasProviderAppService
    {
        Task CompleteVasProviderConfiguration(VasProviderConfigurationDto vasProviderConfigurationDto);
    }
}
