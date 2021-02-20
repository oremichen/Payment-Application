using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO.Service;
using ContentServiceManagementAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContentServiceManagementAPI.Models.DTO.ServiceContentMapping;

namespace ContentServiceManagementAPI.Services.ServiceAppService
{
    public interface IServiceAppService : IDisposable
    {
        Task<IEnumerable<ServiceDto>> GetByServiceProviderId(long serviceProviderId,string search = null);        

        Task <long>Add(Service entity);

        Task<bool> AddServiceOnContentProcessing(Service service);

        Task Update(long id);

        Task<bool> AddServiceCode(long ServiceId, string ServiceCode);

        Task<Service> GetById(long id);

        Task<Service> GetByServiceName(string serviceName);

        Task<IEnumerable<ServiceDtos>> GetByServiceId(long id);
        Task<ServiceDtos> GetServiceByServiceId(long id);

        Task<IEnumerable<ServiceDto>> GetAll(long ApplicationId);

        Task Delete(ServiceDto entity);

        Task<Service> GetbyShortCodekeyword(string shortcode, string keyword);

        Task<List<Service>> GetByShortCodeAndServiceProviderId(string shortCode, long ServiceProviderId);

        Task <IEnumerable<ServiceDto>> GetServicesNotMappedToClientByClientId(int id, long ApplicationId);

        Task<bool> CheckIfNameExistForContentProvider(long serviceProviderId, string name);
        Task<bool> CheckIfNameExist(string name);

        Task<List<string>> ValidateServiceShortCodeOperators(string ShortCode, string OperatorIds);

        Task <bool> UpdateServiceStatus(ServiceDto _service);

        Task<bool> SetServiceProviderServiceStatus(ServiceProviderDto serviceProviderDto, int ServiceStatus);

        Task<ServiceStatusReponseDto> UpdateServiceStatus(ServiceStatusReponseDto ServiceStatusRequest);
        Task<List<Service>> SearchService(ServiceSearchDto serviceSearchDto);
        Task<List<ServiceDto>> GetServicesByServiceProviderId(long ServiceProviderId, int skip, int take);
        int GetServicesByServiceProviderIdCount(long ServiceProviderId);
        Task<List<ServiceContentMapDto>> GetServiceContents(long ServiceId);
        Task<bool> GetContentServiceProviderBalance(long serviceProviderId, long ContentId);
        Task<List<ServiceDto>> GetServicesBySPId(long ServiceProviderId);
        Task<serviceCountDto> GetServicesByServiceProviderIdPaginatedAndCount(long serviceProviderId, int skip = 0, int take = int.MaxValue, string search = null);
    }
}
