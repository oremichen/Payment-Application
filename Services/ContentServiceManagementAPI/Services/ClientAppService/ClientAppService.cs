using ContentServiceManagementAPI.Data;
using ContentServiceManagementAPI.HttpClientService;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.ClientAppService
{
    public class ClientAppService : IClientAppService
    {
        #region Fields
        private readonly ANQContentServiceManageDb _aNQContentServiceManageDb;
        private readonly ILogger<ClientAppService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IAnqClientService _anqClientService;
        #endregion

        #region Ctor
        public ClientAppService( 
            ANQContentServiceManageDb aNQContentServiceManageDb , 
            ILogger<ClientAppService> logger,
            IHttpContextAccessor httpContextAccessor, 
            IAnqClientService anqClientService)
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _anqClientService = anqClientService;
        }
        #endregion

        #region Methods

        #region Add User to Client Table
        
        public async Task<Client> AddClient(Client model)
        {         
            try
            {
                // check if client exist before insertion
                var similarClients = await _aNQContentServiceManageDb.Client.Where(x => x.ContactPersonEmail.Equals(model.ContactPersonEmail) && (x.ClientName.Equals(model.ClientName) || x.AuthenticationId.Equals(model.AuthenticationId))).ToListAsync();
                if(similarClients.Count() < 1)
                {
                    await _aNQContentServiceManageDb.Client.AddAsync(model);
                    await _aNQContentServiceManageDb.SaveChangesAsync();
                    _logger.LogInformation($"User has been added as a Client");
                    return model;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Internal Server Error! Exception Caught");
                return null;
            }
        }
        #endregion

        public async Task<ClientDto> GetClientById(long clientId)
        {
            try
            {
                var clientExist = await _aNQContentServiceManageDb.Client.FindAsync(clientId);
                if (clientExist != null)
                {
                    var clientDto = new ClientDto()
                    {
                        AuthenticationId = clientExist.AuthenticationId,
                        ClientName = clientExist.ClientName,
                        ContactPersonEmail = clientExist.ContactPersonEmail,
                        ContactPersonFirstName = clientExist.ContactPersonFirstName,
                        ContactPersonLastName = clientExist.ContactPersonLastName,
                        ContactPersonPhoneNumber = clientExist.ContactPersonPhoneNumber,
                        ClientId = clientExist.ClientId,
                        AccountEmail = clientExist.AccountEmail,
                        Status = clientExist.Status
                    };
                   return clientDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task<ClientDto> GetClientByAuthId(long authId)
        {
            try
            {
                var clientExist = _aNQContentServiceManageDb.Client.FirstOrDefault(a => a.AuthenticationId == authId);
                if (clientExist != null)
                {
                    var clientDto = new ClientDto()
                    {
                        AuthenticationId = clientExist.AuthenticationId,
                        ClientName = clientExist.ClientName,
                        ContactPersonEmail = clientExist.ContactPersonEmail,
                        ContactPersonFirstName = clientExist.ContactPersonFirstName,
                        ContactPersonLastName = clientExist.ContactPersonLastName,
                        ContactPersonPhoneNumber = clientExist.ContactPersonPhoneNumber,
                        ClientId = clientExist.ClientId,
                        AccountEmail = clientExist.AccountEmail,
                        Status = clientExist.Status
                    };
                    return clientDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task<IEnumerable<ClientDto>> GetAllClients()
        {
            try
            {
                var clienstList = await _aNQContentServiceManageDb.Client.OrderBy(x=>x.ClientId).ToListAsync();
                var clientListDtos = new List<ClientDto>();
                if(clienstList != null)
                {
                    clientListDtos.AddRange(clienstList.OrderBy(c => c.ClientId).Select(x => new ClientDto()
                    {
                        AuthenticationId = x.AuthenticationId,
                        ClientName = x.ClientName,
                        ContactPersonFirstName = x.ContactPersonFirstName,
                        ContactPersonLastName = x.ContactPersonLastName,
                        ContactPersonEmail = x.ContactPersonEmail,
                        ContactPersonPhoneNumber = x.ContactPersonPhoneNumber,
                        ClientId = x.ClientId,
                        AccountEmail = x.AccountEmail,
                        Status = x.Status,
                        ApplicationId = x.ApplicationId
                    }));
                   
                    return clientListDtos;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;                
            }
        }


        public async Task<ClientDto> UpdateAccountSetup(ClientDto clientDto)
        {
            try
            {
                var _client = _aNQContentServiceManageDb.Client.FirstOrDefault(a => a.AuthenticationId == clientDto.AuthenticationId);
                _client.ClientName = clientDto.ClientName;
                _client.ContactPersonEmail = clientDto.ContactPersonEmail;
                _client.ContactPersonFirstName = clientDto.ContactPersonFirstName;
                _client.ContactPersonLastName = clientDto.ContactPersonLastName;
                _client.AccountEmail = clientDto.ContactPersonEmail;
                _aNQContentServiceManageDb.Client.Update(_client);
                int count = _aNQContentServiceManageDb.SaveChanges();
                if(count > 0)
                {
                    return clientDto;
                }
                return null;
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task UpdateDndClient(DndClientUpdateRequest dndClientUpdateRequest)
        {
            try
            {
                var Client = _aNQContentServiceManageDb.Client.FirstOrDefault(a => a.ClientId == dndClientUpdateRequest.ClientId);
                if(Client != null)
                {
                    Client.ContactPersonFirstName = dndClientUpdateRequest.FirstName;
                    Client.ContactPersonLastName = dndClientUpdateRequest.LastName;
                    Client.ContactPersonEmail = dndClientUpdateRequest.EmailAddress;

                    _aNQContentServiceManageDb.Client.Update(Client);
                    _aNQContentServiceManageDb.SaveChanges();
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) {
                 _aNQContentServiceManageDb.Database.CloseConnection();        
                }            
                disposedValue = true;
            }
        }
    // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
           
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
