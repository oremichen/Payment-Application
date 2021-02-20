using ContentServiceManagementAPI.HttpClientService.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.HttpClientService
{
    public interface IAnqClientService : IDisposable
    {
        Task <List<AnqClientDto>> GetAllClients();
        Task <List<AnqContentProviderDto>>GetAllContentProviders();
        Task<List<AnqPartnerDto>> GetAllServiceProviders();
        Task<List<AnqCommandRecordDto>> GetAnqCommandRecords(int clientId);

    }
}
