using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO.ServiceMappingDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services
{
    public interface IMapServiceToClientAppService 
    {
        Task Add(MapServiceToClient entity);

        Task<IEnumerable<MappedServiceDto>> GetAll();
        Task<IEnumerable<MappedServiceDto>> GetAllByServiceProviderId(long serviceProviderId);
        Task<IEnumerable<MappedServiceDto>> GetAllByClientId(long clientId);
        Task<IEnumerable<MappedServiceDto>> GetAllByServiceId(long serviceId);
        
        Task<MappedServiceDto> GetMappedServiceById(long id);

        Task Update(MappedServiceDto entity);
        Task Delete(MappedServiceDto entity);

        Task<bool> HasServiceBeenMapped(long serviceId , long clientId);
        Task SaveAsync();
        Task<ClientSkipTakeAndCountModelDto> GetAllMappedClientToSpServicesByserviceProviderId(long id, int skip, int take);
        Task<ClientSkipTakeAndCountModelDto> GetAllMappedClientToSpServicesByserviceProviderIAndSearch(long id, string search = null);
        Task<List<Client>> GetAllMappedClientToSpServicesByServiceIdDropdown(long id);

        bool HasServiceRateBeenMapped(long rateId);


    }
}
