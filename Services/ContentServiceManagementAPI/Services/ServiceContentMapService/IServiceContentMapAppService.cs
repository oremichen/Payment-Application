using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO.ServiceContentMapping;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.ServiceContentMapService
{
     public interface IServiceContentMapAppService : IDisposable
    {
        Task<long> AddSync(CreateServiceContentMapDto model);

        Task Delete(ServiceContentMapDto model);

        Task<long> Update(ServiceContentMapp model);

        Task<IEnumerable<ServiceContentMapp>> GetAllByServiceId(long serviceId);
        Task<IEnumerable<ServiceContentMapServiceNameDto>> GetAllByContentId(long contentId);

        Task<IEnumerable<ServiceContentMapp>> GetAll();
        Task<ServiceContentMapp> GetById(long id);

        Task SaveAsync();
        Task<bool> CheckIfContentBeenMappedToService(long serviceId, long contentId);
       
    }
}
