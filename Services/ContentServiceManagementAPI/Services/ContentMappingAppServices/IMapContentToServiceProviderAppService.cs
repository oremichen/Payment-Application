using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DTO;
using ContentServiceManagementAPI.Models.DTO.ContentMappingDto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services
{
    public interface IMapContentToServiceProviderAppService : IDisposable
    {
        Task Add(AddMapContentDto entity);
        Task<IEnumerable<MappedContentDto>> GetAll();
        Task<MappedContentDto> GetById(long id);
        Task Update(MappedContentDto entity);
        Task Delete(MappedContentDto entity);

        Task<IEnumerable<MappedContentDto>> GetMappedContentsForContentProvider(long contentProviderId);
        Task<IEnumerable<ServiceProvider>> GetAllByContentProviderId(long id);
        Task<IEnumerable<MappedContentDto>> GetMapedContentsForServiceProvider(long serviceProviderId);
        Task<IEnumerable<MappedContentDto>> GetAllByContentId(long contentId);
        Task<bool> HasContentBeenMapped(long contentId, long serviceProviderId);
        Task<IEnumerable<MappedContentDto>> GetAllServiceProviderMappedToAContent(long contentId, long serviceProviderId);
        Task<IEnumerable<MappedContentDto>> GetAllSpMappedToAContent(long serviceProviderId);
        Task<MappedContentDto> GetAllMappedContentByMapContentToServiceProviderId(long mapContentToServiceProviderId);
        Task<int?> GetServicesMappedToContentByContentId(long contentId);
        Task<IEnumerable<ServiceProvider>> GetAllSPByCPIdAndContentId(long contentProviderId, long contentId);
        Task<IEnumerable<ServiceProviderDto>> GetDistinctSpforCpByContentProviderId(long id);
        Task<long> GetSpByContentProviderId(long id);

        bool HasContentRateBeenMapped(long rateId);

    }
}
