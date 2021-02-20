using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.ContentProviderService
{
    public interface IContentProviderAppService
    {
        Task<ContentProvider> AddContentProvider(ContentProvider item);

        Task<IEnumerable<ContentProviderDto>> GetContentProviders();

        Task<ContentProviderDto> GetContentProvider(long contentProvider);

        Task UpdateCommandRecord(int clientId);

        Task<List<CommandRecord>> GetCommandRecordsByClientId(int clientId);

        Task<ContentProvider> GetContentProviderIdByAuthId(long authId);
        Task<ContentProviderDto> GetContentProviderByAuthId(long authId);
        

    }
}
