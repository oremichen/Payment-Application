using Anq.Enums;
using ContentServiceManagementAPI.Data;
using ContentServiceManagementAPI.Helpers;
using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO.ServiceMappingDto;
using ContentServiceManagementAPI.Services.ServiceAppService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services
{
    public class MapServiceToClientAppService : IMapServiceToClientAppService
    {
        #region Fields
        private ANQContentServiceManageDb _aNQContentServiceManageDb;
        private IHttpContextAccessor _httpContextAccessor;
        private ILogger<MapServiceToClientAppService> _logger;
        private IServiceAppService _serviceAppService;
       
        #endregion 

        #region ctr
        public MapServiceToClientAppService(
            ANQContentServiceManageDb aNQContentServiceManageDb, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<MapServiceToClientAppService> logger)
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
       
        #endregion

        public async Task Add(MapServiceToClient model)
        {
            await _aNQContentServiceManageDb.MapServiceToClient.AddAsync(model);
        }

        public async Task<IEnumerable<MappedServiceDto>> GetAll()
        {
            try
            {
                var mappedServices = await _aNQContentServiceManageDb.MapServiceToClient.ToListAsync();
                if(mappedServices != null)
                {
                    var mappedServicesDto = new List<MappedServiceDto>();

                    mappedServicesDto.AddRange(mappedServices.OrderBy(x => x.MapServiceToClientId).Select(x => new MappedServiceDto()
                    {
                        MappedServiceId = x.MapServiceToClientId,
                        ServiceId = x.ServiceId,
                        ClientId = x.ClientId,
                        ServiceProviderId = x.ServiceProviderId,
                        ServiceProviderName = x.ServiceProviderName,
                        ServiceName =x.ServiceName,
                        CreatedOn = x.CreatedOn,
                        RateId = x.RateId,
                        RateName = x.RateName,
                        MappedBy = x.MappedBy,
                        DedicatedRateId=x.DedicatedRateId,
                        DedicatedRateName=x.DedicatedRateName,
                        RateType=x.RateType,
                        RateCurrency = (CurrencyEnum)x.RateCurrency,
                        IsPublished = x.IsPublished
                    }));

                    return mappedServicesDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }

        public async Task<IEnumerable<MappedServiceDto>> GetAllByServiceProviderId(long serviceProviderId)
        {
            try
            {
                var mappedServices = await _aNQContentServiceManageDb.MapServiceToClient.Where(x => x.ServiceProviderId == serviceProviderId).ToListAsync();
                if(mappedServices != null)
                {
                    var mappedServicesDto = new List<MappedServiceDto>();

                    mappedServicesDto.AddRange(mappedServices.OrderBy(x => x.MapServiceToClientId).Select(x => new MappedServiceDto()
                    {
                        MappedServiceId = x.MapServiceToClientId,
                        ServiceId = x.ServiceId,
                        ClientId = x.ClientId,
                        ServiceProviderId = x.ServiceProviderId,
                        ServiceProviderName = x.ServiceProviderName,
                        CreatedOn = x.CreatedOn,
                        RateId = x.RateId,
                        ServiceName = x.ServiceName,
                        RateName = x.RateName,
                        MappedBy = x.MappedBy,
                        DedicatedRateId = x.DedicatedRateId,
                        DedicatedRateName = x.DedicatedRateName,
                        RateType = x.RateType,
                        RateCurrency = (CurrencyEnum)x.RateCurrency,
                    }));

                    return mappedServicesDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }

        public async Task<IEnumerable<MappedServiceDto>> GetAllByClientId(long clientId)
        {
            try
            {
                var mappedServices = await _aNQContentServiceManageDb.MapServiceToClient.Where(x => x.ClientId == clientId).OrderBy(or => or.CreatedOn).ToListAsync();
                if(mappedServices != null)
                {
                    var mappedServicesDto = new List<MappedServiceDto>();

                    mappedServicesDto.AddRange(mappedServices.OrderBy(x => x.MapServiceToClientId).Select(x => new MappedServiceDto()
                    {
                        MappedServiceId = x.MapServiceToClientId,
                        ServiceId = x.ServiceId,
                        ClientId = x.ClientId,
                        ServiceProviderId = x.ServiceProviderId,
                        ServiceProviderName = x.ServiceProviderName,
                        ServiceName = x.ServiceName,
                        CreatedOn = x.CreatedOn,
                        RateId = x.RateId,
                        RateName = x.RateName,
                        MappedBy = x.MappedBy,
                        DedicatedRateId = x.DedicatedRateId,
                        DedicatedRateName = x.DedicatedRateName,
                        RateType = x.RateType,
                        RateCurrency = (CurrencyEnum)x.RateCurrency,
                    }));

                    return mappedServicesDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }

        public async Task<IEnumerable<MappedServiceDto>> GetAllByServiceId(long serviceId)
        {
            try
            {
                var mappedServices = await _aNQContentServiceManageDb.MapServiceToClient.Where(x => x.ServiceId == serviceId).OrderByDescending(or => or.CreatedOn).ToListAsync();
               if(mappedServices != null){
                    var mappedServicesDto = new List<MappedServiceDto>();
                    mappedServicesDto.AddRange(mappedServices.OrderBy(x => x.MapServiceToClientId).Select(x => new MappedServiceDto()
                    {
                        MappedServiceId = x.MapServiceToClientId,
                        ServiceId = x.ServiceId,
                        ClientId = x.ClientId,
                        ServiceProviderId = x.ServiceProviderId,
                        ServiceProviderName= x.ServiceProviderName,
                        ServiceName = x.ServiceName,
                        CreatedOn = x.CreatedOn,
                        RateId = x.RateId,
                        RateName = x.RateName,
                        MappedBy = x.MappedBy,
                        DedicatedRateId = x.DedicatedRateId,
                        DedicatedRateName = x.DedicatedRateName,
                        RateType = x.RateType,
                        RateCurrency =(CurrencyEnum)x.RateCurrency,
                    }));
                    return mappedServicesDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }
        public async Task<MappedServiceDto> GetMappedServiceById (long mappedServiceId)
        {
            try
            {
                var retrievedEntity = await _aNQContentServiceManageDb.MapServiceToClient.AsNoTracking().FirstOrDefaultAsync(x=>x.MapServiceToClientId == mappedServiceId);
                if (retrievedEntity != null)
                {
                    var mappService = new MappedServiceDto()
                    {
                        ServiceId = retrievedEntity.ServiceId,
                        MappedServiceId = retrievedEntity.MapServiceToClientId,
                        ClientId = retrievedEntity.ClientId,
                        ServiceProviderId = retrievedEntity.ServiceProviderId,
                        ServiceProviderName = retrievedEntity.ServiceProviderName,
                        ServiceName= retrievedEntity.ServiceName,
                        CreatedOn = retrievedEntity.CreatedOn,
                        RateId = retrievedEntity.RateId,
                        RateName = retrievedEntity.RateName,
                        MappedBy = retrievedEntity.MappedBy,
                        DedicatedRateId = retrievedEntity.DedicatedRateId,
                        DedicatedRateName = retrievedEntity.DedicatedRateName,
                        RateType = retrievedEntity.RateType,
                        RateCurrency = (CurrencyEnum)retrievedEntity.RateCurrency,
                    };
                    return mappService;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }  

        public async Task Update(MappedServiceDto model)
        {
            try
            {
                var service = await this.GetMappedServiceById(model.MappedServiceId);
                if (service != null)
                {
                    var mappedService = new MapServiceToClient()
                    {
                        ServiceId = model.ServiceId,
                        MapServiceToClientId = model.MappedServiceId,
                        ClientId = model.ClientId,
                        ServiceProviderId = model.ServiceProviderId,
                        ServiceProviderName = model.ServiceProviderName,
                        ServiceName = model.ServiceName,
                        CreatedOn = model.CreatedOn,
                        RateId = model.RateId,
                        RateName = model.RateName,
                        MappedBy = model.MappedBy,
                        DedicatedRateId = model.DedicatedRateId,
                        DedicatedRateName = model.DedicatedRateName,
                        RateType = model.RateType,
                        RateCurrency = (int)model.RateCurrency,
                        IsPublished = model.IsPublished
                    };
                    _aNQContentServiceManageDb.Entry(mappedService).State = EntityState.Modified;
                    await SaveAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
            
        }

        public async Task Delete (MappedServiceDto model)
        {
            try
            {
                var content = await _aNQContentServiceManageDb.MapServiceToClient.AsNoTracking().FirstOrDefaultAsync(f=>f.MapServiceToClientId == model.MappedServiceId);
                if(content != null)
                {            
                    _aNQContentServiceManageDb.MapServiceToClient.Remove(content);
                    await this.SaveAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }


        public async Task<ClientSkipTakeAndCountModelDto> GetAllMappedClientToSpServicesByserviceProviderId(long id, int skip, int take)
        {
            try
            {
                var countClients = await _aNQContentServiceManageDb.MapServiceToClient.Where(x => x.ServiceProviderId == id).ToListAsync();
                List<Client> clients = new List<Client>();
                var getAllBySPId = new List<MapServiceToClient>();

                getAllBySPId = await _aNQContentServiceManageDb.MapServiceToClient.
                    Where(x => x.ServiceProviderId == id).ToListAsync();

                var getAllByServiceProviderId = getAllBySPId.Distinct(new MappedServiceToClientComparer()).Skip(skip).Take(take).OrderByDescending(a => a.CreatedOn).ToList();

                var client = new Client();
                foreach (var map in getAllByServiceProviderId)
                {
                    client = _aNQContentServiceManageDb.Client.FirstOrDefault(a => a.ClientId == map.ClientId);

                    if (client != null)
                        clients.Add(client);
                }

                //r  }eturn clients.Distinct().Skip(skip).Take(take);
                var model = new ClientSkipTakeAndCountModelDto
                {
                    ClientModel = clients,
                    CountClients = countClients.Distinct(new MappedServiceToClientComparer()).Count()
                };

                return model;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
                
          
        }

        public async Task<ClientSkipTakeAndCountModelDto> GetAllMappedClientToSpServicesByserviceProviderIAndSearch(long id, string search = null)
        {
            try
            {
                long countClients = _aNQContentServiceManageDb.MapServiceToClient.Count();
                List<Client> clients = new List<Client>();
                var getAllByContentProviderId = new List<MapServiceToClient>();
                var getAllMappedUsers = new ClientSkipTakeAndCountModelDto();
                if (search == null)
                {
                    getAllMappedUsers = await GetAllMappedClientToSpServicesByserviceProviderId(id, 0, 5);

                    //getAllByContentProviderId = await _aNQContentServiceManageDb.MapServiceToClient.
                    //    Where(x => x.ServiceProviderId == id).OrderByDescending(a => a.CreatedOn).ToListAsync();
                }
                else if (search != null)
                {
                    getAllByContentProviderId = await _aNQContentServiceManageDb.MapServiceToClient.
                        Where(x => x.ServiceProviderId == id && x.ClientName.ToLower().Contains(search)).OrderByDescending(a => a.CreatedOn).ToListAsync();

                }
                var client = new Client();
                foreach (var map in getAllByContentProviderId)
                {
                    client = _aNQContentServiceManageDb.Client.FirstOrDefault(a => a.ClientId == map.ClientId);

                    if (client != null)
                        clients.Add(client);
                }

                //r  }eturn clients.Distinct().Skip(skip).Take(take);
                var model = new ClientSkipTakeAndCountModelDto
                {
                    ClientModel = clients.Distinct(),
                    CountClients = countClients
                };

                return model;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
            

        }

        public async Task<List<Client>> GetAllMappedClientToSpServicesByServiceIdDropdown(long id)
        {
            try
            {
                List<Client> clients = new List<Client>();
                var getAllBySPId = new List<MapServiceToClient>();

                //getAllBySPId = await _aNQContentServiceManageDb.MapServiceToClient.
                //    Where(x => x.ServiceId == id).ToListAsync();
                getAllBySPId = await _aNQContentServiceManageDb.MapServiceToClient.
                   Where(x => x.ServiceProviderId == id).ToListAsync();

                var getAllByServiceProviderId = getAllBySPId.Distinct(new MappedServiceToClientComparer()).ToList();

                var client = new Client();
                foreach (var map in getAllByServiceProviderId)
                {
                    client = _aNQContentServiceManageDb.Client.FirstOrDefault(a => a.ClientId == map.ClientId);

                    if (client != null)
                        clients.Add(client);
                }


                return clients;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }


        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public async Task SaveAsync()
        {
            await _aNQContentServiceManageDb.SaveChangesAsync();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        _aNQContentServiceManageDb.Database.CloseConnection();
                    }
                    disposedValue = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }
        public void Dispose()
        {
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
            GC.CancelFullGCNotification();
        }

        #endregion

        #region Checks
        public async Task<bool> HasServiceBeenMapped(long serviceId , long clientId) 
        {
            try
            {
                var hasItBeenMapped = await _aNQContentServiceManageDb.MapServiceToClient
                .FirstOrDefaultAsync(x => x.ServiceId == serviceId && x.ClientId == clientId);
                if (hasItBeenMapped == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
            
        }

        public bool HasServiceRateBeenMapped(long rateId)
        {
            var rate = _aNQContentServiceManageDb.MapServiceToClient
               .FirstOrDefault(ex => ex.RateId == rateId);
            if (rate != null)
            {
                return true;
            }
            return false;
        }

        #endregion


    }
}
