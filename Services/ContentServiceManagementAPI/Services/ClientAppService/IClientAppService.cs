using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.ClientAppService
{
    public interface IClientAppService : IDisposable
    {
        Task<Client> AddClient(Client model);
        Task<IEnumerable<ClientDto>> GetAllClients();
        Task<ClientDto> GetClientById(long id);
        Task<ClientDto> GetClientByAuthId(long authId);
        Task<ClientDto> UpdateAccountSetup(ClientDto clientDto);
        Task UpdateDndClient(DndClientUpdateRequest dndClientUpdateRequest);
    }
}
