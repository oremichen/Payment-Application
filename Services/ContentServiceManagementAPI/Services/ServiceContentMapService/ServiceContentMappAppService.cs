using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentServiceManagementAPI.Data;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO.ServiceContentMapping;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContentServiceManagementAPI.Services.ServiceContentMapService
{
    public class ServiceContentMappAppService : IServiceContentMapAppService
    {

        private ANQContentServiceManageDb _aNQContentServiceManageDb;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ServiceContentMappAppService> _logger;

        public ServiceContentMappAppService(ANQContentServiceManageDb aNQContentServiceManageDb,
            IHttpContextAccessor httpContextAccessor, ILogger<ServiceContentMappAppService> logger)
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<long> AddSync(CreateServiceContentMapDto model)
        {
            try
            {
                var data = new ServiceContentMapp()
                {
                    ServiceId = model.ServiceId,
                    ContentId = model.ContentId,
                    ServiceProviderId = model.ServiceProviderId,
                    CreatedOn = DateTime.UtcNow,
                };
                await _aNQContentServiceManageDb.ServiceContentMap.AddAsync(data);
                    return data.ServiceContentMappId;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw;
            }
        }

        public async Task<long> Update(ServiceContentMapp model)
        {
            try
            {
                var update = await _aNQContentServiceManageDb.ServiceContentMap.FindAsync(model.ServiceContentMappId);
                _aNQContentServiceManageDb.Entry(update).State = EntityState.Modified;
                _logger.LogInformation($"Update of the enitity with id {model.ServiceContentMappId} was successfull");

                await this.SaveAsync();
                return model.ServiceContentMappId;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }

        public async Task<IEnumerable<ServiceContentMapp>> GetAll()
        {
            try
            {
                var data = await _aNQContentServiceManageDb.ServiceContentMap.OrderByDescending(or => or.CreatedOn).ToListAsync();
                if (data.Count() > 0 || data != null)
                {
                    _logger.LogInformation($"Success!{data.Count()} map contentService returned");
                    return data;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }

        public async Task <ServiceContentMapp> GetById(long id)
        {
            try
            {
                var data = await _aNQContentServiceManageDb.ServiceContentMap.FindAsync(id);
                if (data != null)
                {
                    _logger.LogInformation($"Success! item found and returned");
                    return data;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }

        public async Task<IEnumerable<ServiceContentMapp>> GetAllByServiceId(long serviceId)
        {
            try
            {
                var data = await _aNQContentServiceManageDb.ServiceContentMap.Where(s => s.ServiceId == serviceId).OrderByDescending(or=>or.CreatedOn).ToListAsync();
                if (data != null)
                {
                    _logger.LogInformation($"Success!{data.Count()} map content Services returned");
                    return data;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }
        public async Task<IEnumerable<ServiceContentMapServiceNameDto>> GetAllByContentId(long contentId)
        {
            try
            {
                var data = await _aNQContentServiceManageDb.ServiceContentMap.Where(s => s.ContentId == contentId).OrderByDescending(or=>or.CreatedOn)
                    .Select(y => new ServiceContentMapServiceNameDto { 
                        ContentId = y.ContentId,
                        ServiceId = y.ServiceId,
                        ServiceName = _aNQContentServiceManageDb.Service.Where(x => x.ServiceId == y.ServiceId).Select(r => r.ServiceName).FirstOrDefault(),
                        ServiceProviderId = y.ServiceProviderId,
                        CreatedOn = y.CreatedOn
                    }).ToListAsync();
                if (data != null)
                {
                    _logger.LogInformation($"Success!{data.Count()} map content Services returned");
                    return data;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        public async Task Delete(ServiceContentMapDto model)
        {
            try
            {
                var serviceToDelete = await _aNQContentServiceManageDb.ServiceContentMap.FindAsync(model.ServiceContentMapId);
                if (serviceToDelete != null)
                {
                    _aNQContentServiceManageDb.ServiceContentMap.Remove(serviceToDelete);
                    await this.SaveAsync();
                    _logger.LogInformation($"AppService Deleted a service with name {model.ServiceContentMapId}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
            }
        }

        public async Task SaveAsync()
        {
            await _aNQContentServiceManageDb.SaveChangesAsync();
        }

        #region Validation
        public async Task<bool> CheckIfContentBeenMappedToService(long serviceId, long contentId)
        {
            try
            {
                var check = await _aNQContentServiceManageDb.ServiceContentMap.Where(x => x.ServiceId == serviceId
                                    && x.ContentId == contentId ).FirstOrDefaultAsync();
                if (check != null)
                    return true;

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw;
            }
        }
        #endregion


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
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

       
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
           
        }
        #endregion
    }
}
