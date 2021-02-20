using Anq.Enums;
using ContentServiceManagementAPI.Data;
using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DTO;
using ContentServiceManagementAPI.Models.DTO.ContentMappingDto;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services
{
    public class MapContentToServiceProviderAppService : IMapContentToServiceProviderAppService
    {
        private ANQContentServiceManageDb _aNQContentServiceManageDb;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<MapContentToServiceProviderAppService> _logger;


        public MapContentToServiceProviderAppService(ANQContentServiceManageDb aNQContentServiceManageDb, IHttpContextAccessor httpContextAccessor,
            ILogger<MapContentToServiceProviderAppService> logger)
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task Add(AddMapContentDto model)
        {
            try
            {
                var mappedContent = new MapContentToServiceProvider()
                {
                    ContentId = model.ContentId,
                    ContentProviderId = model.ContentProviderId,
                    ContentProviderName = model.ContentProviderName,
                    ServiceProviderId = model.ServiceProviderId,
                    CreatedOn = DateTime.UtcNow,
                    MappedBy = model.MappedBy,
                    RateId = model.RateId,
                    RateName = model.RateName,
                    DedicatedRateId = model.DedicatedRateId,
                    DedicatedRateName = model.DedicatedRateName,
                    RateType = model.RateType,
                    RateCurrency = (CurrencyEnum)model.RateCurrency,
                    IsPublished = false
                };
                await _aNQContentServiceManageDb.MapContentToServiceProvider.AddAsync(mappedContent);
                await this.SaveAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public async Task<IEnumerable<MappedContentDto>> GetAll()
        {
            try
            {
                var mappedContents = await _aNQContentServiceManageDb.MapContentToServiceProvider.OrderByDescending(or=>or.CreatedOn).ToListAsync();
                var mappedContentsDto = new List<MappedContentDto>();
                mappedContentsDto.AddRange(mappedContents.OrderBy(x => x.MapContentToServiceProviderId).Select(x => new MappedContentDto()
                {
                    MapContentToServiceProviderId = x.MapContentToServiceProviderId,
                    ContentId = x.ContentId,
                    ContentProviderId = x.ContentProviderId,
                    ContentProviderName = x.ContentProviderName,
                    ServiceProviderId = x.ServiceProviderId,
                    CreatedOn = x.CreatedOn,
                    RateId = x.RateId,
                    RateName = x.RateName,
                    MappedBy = x.MappedBy,
                    DedicatedRateId = x.DedicatedRateId,
                    DedicatedRateName = x.DedicatedRateName,
                    RateType = x.RateType,
                    RateCurrency = x.RateCurrency,
                    IsPublished = x.IsPublished
                    
                }));

                return mappedContentsDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        public async Task<IEnumerable<MappedContentDto>> GetAllServiceProviderMappedToAContent(long contentId, long serviceProviderId)
        {
            try
            {
                var mappedContents = await _aNQContentServiceManageDb.MapContentToServiceProvider.Where(x=>x.ContentId==contentId && x.ServiceProviderId==serviceProviderId).ToListAsync();
                var mappedContentsDto = new List<MappedContentDto>();
                mappedContentsDto.AddRange(mappedContents.OrderBy(x => x.MapContentToServiceProviderId).Select(x => new MappedContentDto()
                {
                    MapContentToServiceProviderId = x.MapContentToServiceProviderId,
                    ContentId = x.ContentId,
                    ContentProviderId = x.ContentProviderId,
                    ContentProviderName = x.ContentProviderName,
                    ServiceProviderId = x.ServiceProviderId,
                    CreatedOn = x.CreatedOn,
                    RateId = x.RateId,
                    RateName = x.RateName,
                    MappedBy = x.MappedBy,
                    DedicatedRateId = x.DedicatedRateId,
                    DedicatedRateName = x.DedicatedRateName,
                    RateType = x.RateType,
                    RateCurrency = x.RateCurrency,
                    IsPublished = x.IsPublished
                }));

                return mappedContentsDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        public async Task <MappedContentDto> GetById(long id)
        {
            try
            {
                var retrievedEntity = await _aNQContentServiceManageDb.MapContentToServiceProvider.AsNoTracking().FirstOrDefaultAsync(x => x.MapContentToServiceProviderId == id);
                if (retrievedEntity != null)
                {
                    var mappedContent = new MappedContentDto()
                    {
                        ContentId = retrievedEntity.ContentId,
                        ContentProviderName = retrievedEntity.ContentProviderName,
                        MapContentToServiceProviderId = retrievedEntity.MapContentToServiceProviderId,
                        ContentProviderId = retrievedEntity.ContentProviderId,
                        ServiceProviderId = retrievedEntity.ServiceProviderId,
                        CreatedOn = retrievedEntity.CreatedOn,
                        RateId = retrievedEntity.RateId,
                        RateName = retrievedEntity.RateName,
                        MappedBy = retrievedEntity.MappedBy,
                        DedicatedRateId = retrievedEntity.DedicatedRateId,
                        DedicatedRateName = retrievedEntity.DedicatedRateName,
                        RateType = retrievedEntity.RateType,
                        RateCurrency = retrievedEntity.RateCurrency,
                    };
                    return mappedContent;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
            
        }

        public async Task Update(MappedContentDto model)
        {
            try
            {
                var content = await this.GetById(model.MapContentToServiceProviderId);
                if (content != null)
                {
                    var mappedContent = new MapContentToServiceProvider()
                    {
                        ContentId = model.ContentId,
                        ContentProviderName = model.ContentProviderName,
                        MapContentToServiceProviderId = model.MapContentToServiceProviderId,
                        ContentProviderId = model.ContentProviderId,
                        ServiceProviderId = model.ServiceProviderId,
                        CreatedOn = model.CreatedOn,
                        RateId = model.RateId,
                        RateName = model.RateName,
                        MappedBy = model.MappedBy,
                        DedicatedRateId = model.DedicatedRateId,
                        DedicatedRateName = model.DedicatedRateName,
                        RateType = model.RateType,
                        RateCurrency = model.RateCurrency,
                        IsPublished = model.IsPublished

                    };
                    _aNQContentServiceManageDb.Entry(mappedContent).State = EntityState.Modified;
                    await SaveAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        
        }

        public async Task Delete(MappedContentDto model)
        {
            try
            {
                var content = await _aNQContentServiceManageDb.MapContentToServiceProvider.AsNoTracking().FirstOrDefaultAsync(x => x.MapContentToServiceProviderId == model.MapContentToServiceProviderId);
                var mapToDelete = new MapContentToServiceProvider()
                {
                    ContentId = content.ContentId,
                    ContentProviderName = content.ContentProviderName,
                    MapContentToServiceProviderId = content.MapContentToServiceProviderId,
                    ContentProviderId = content.ContentProviderId,
                    ServiceProviderId = content.ServiceProviderId,
                    CreatedOn = content.CreatedOn,
                    RateName = content.RateName,
                    RateId = content.RateId,
                    DedicatedRateId = content.DedicatedRateId,
                    DedicatedRateName = content.DedicatedRateName,
                    RateType = content.RateType,
                    RateCurrency = content.RateCurrency,

                    MappedBy = content.MappedBy
                };
                _aNQContentServiceManageDb.MapContentToServiceProvider.Remove(mapToDelete);
                await SaveAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
            
        }

        public async Task<IEnumerable<MappedContentDto>> GetMappedContentsForContentProvider(long id)
        {
            try
            {
                var mappedContents = await _aNQContentServiceManageDb.MapContentToServiceProvider.AsNoTracking().
                                Where(x => x.ContentProviderId == id).OrderByDescending(or => or.CreatedOn).ToListAsync();
                if (mappedContents != null)
                {
                    var mappedContentsDto = new List<MappedContentDto>();

                    mappedContentsDto.AddRange(mappedContents.OrderByDescending(x => x.CreatedOn).Select(x => new MappedContentDto()
                    {
                        MapContentToServiceProviderId = x.MapContentToServiceProviderId,
                        ContentId = x.ContentId,
                        ContentProviderId = x.ContentProviderId,
                        ContentProviderName = x.ContentProviderName,
                        ServiceProviderId = x.ServiceProviderId,
                        CreatedOn = x.CreatedOn,
                        RateId = x.RateId,
                        RateName = x.RateName,
                        MappedBy = x.MappedBy,
                        DedicatedRateId = x.DedicatedRateId,
                        DedicatedRateName = x.DedicatedRateName,
                        RateType = x.RateType,
                        RateCurrency = x.RateCurrency,
                    }));

                    return mappedContentsDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
     
        }

        // working to use DTo
        public async Task<IEnumerable<MappedContentDto>> GetMapedContentsForServiceProvider(long id)
        {
            try
            {
                var mappedContents = await _aNQContentServiceManageDb.MapContentToServiceProvider.AsNoTracking().
                                 Where(x => x.ServiceProviderId == id).OrderByDescending(or => or.CreatedOn).ToListAsync();

                var sps = _aNQContentServiceManageDb.ServiceProvider.FirstOrDefault(a => a.ServiceProviderId == id);


                if (mappedContents != null)
                {

                    var mappedContentsDto = new List<MappedContentDto>();
                    mappedContentsDto.AddRange(mappedContents.OrderByDescending(x => x.CreatedOn).Select(x => new MappedContentDto()
                    {
                        MapContentToServiceProviderId = x.MapContentToServiceProviderId,
                        ContentId = x.ContentId,
                        ContentProviderId = x.ContentProviderId,
                        ServiceProviderId = x.ServiceProviderId,
                        ContentProviderName = x.ContentProviderName,
                        CreatedOn = x.CreatedOn,
                        RateId = x.RateId,
                        RateName = x.RateName,
                        MappedBy = x.MappedBy,
                        DedicatedRateId = x.DedicatedRateId,
                        DedicatedRateName = x.DedicatedRateName,
                        RateType = x.RateType,
                        RateCurrency = x.RateCurrency,
                        ServiceProviderName = sps.ServiceProviderName
                    }));
                    return mappedContentsDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
            
        }


        public async Task<IEnumerable<MappedContentDto>> GetAllSpMappedToAContent(long serviceProviderId)
        {
            try
            {
                List<MappedContentDto> mappedContent = new List<MappedContentDto>();

                var records = await _aNQContentServiceManageDb.MapContentToServiceProvider.
                     Where(x => x.ServiceProviderId == serviceProviderId).OrderByDescending(or => or.CreatedOn).ToListAsync();
                var serviceproviderName = "";
                foreach (var item in records)
                {
                   var sps = _aNQContentServiceManageDb.ServiceProvider.FirstOrDefault(a => a.ServiceProviderId == item.ServiceProviderId);
                    mappedContent.Add(new MappedContentDto
                    {
                        MapContentToServiceProviderId = item.MapContentToServiceProviderId,
                        ContentProviderId = item.ContentProviderId,
                        ContentProviderName = item.ContentProviderName,
                        ContentId = item.ContentId,
                        ServiceProviderId = item.ServiceProviderId,
                        CreatedOn = item.CreatedOn,
                        MappedBy = item.MappedBy,
                        RateCurrency = item.RateCurrency,
                        RateId = item.RateId,
                        RateName = item.RateName,
                        DedicatedRateId = item.DedicatedRateId,
                        RateType = item.RateType,
                        IsPublished = item.IsPublished,
                        ServiceProviderName = sps.ServiceProviderName
                    });
                    serviceproviderName = sps.ServiceProviderName;
                }
                return mappedContent;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        public async Task<IEnumerable<MappedContentDto>> GetAllByContentId(long id)
        {
            try
            {
                var mappedContents = await _aNQContentServiceManageDb.MapContentToServiceProvider.AsNoTracking().
                                Where(x => x.ContentId == id).OrderByDescending(or => or.CreatedOn).ToListAsync();
                if (mappedContents != null)
                {
                    var mappedContentsDto = new List<MappedContentDto>();
                    mappedContentsDto.AddRange(mappedContents.OrderByDescending(x => x.CreatedOn).Select(x => new MappedContentDto()
                    {
                        MapContentToServiceProviderId = x.MapContentToServiceProviderId,
                        ContentId = x.ContentId,
                        ContentProviderId = x.ContentProviderId,
                        ServiceProviderId = x.ServiceProviderId,
                        ContentProviderName = x.ContentProviderName,
                        CreatedOn = x.CreatedOn,
                        RateId = x.RateId,
                        RateName = x.RateName,
                        MappedBy = x.MappedBy,
                        DedicatedRateId = x.DedicatedRateId,
                        DedicatedRateName = x.DedicatedRateName,
                        RateType = x.RateType,
                        RateCurrency = x.RateCurrency,
                    }));
                    return mappedContentsDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public async Task<IEnumerable<ServiceProvider>> GetAllByContentProviderId(long id)
        {
            try
            {
                List<ServiceProvider> serviceProviders = new List<ServiceProvider>();
                var getAllByContentProviderId = await _aNQContentServiceManageDb.MapContentToServiceProvider.
                    Where(x => x.ContentProviderId == id).OrderByDescending(a => a.CreatedOn).ToListAsync();

                foreach (var map in getAllByContentProviderId)
                {
                    var serviceProvider = _aNQContentServiceManageDb.ServiceProvider.FirstOrDefault(a => a.ServiceProviderId == map.ServiceProviderId);

                    if (serviceProvider != null)
                        serviceProviders.Add(serviceProvider);
                }

                return serviceProviders;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
                               
        }

        
        public async Task<long> GetSpByContentProviderId(long id)
        {
            try
            {
                List<ServiceProvider> serviceProviders = new List<ServiceProvider>();
                var getAllByContentProviderId = await _aNQContentServiceManageDb.MapContentToServiceProvider.
                    Where(x => x.ContentProviderId == id).OrderByDescending(a => a.CreatedOn).ToListAsync();

                foreach (var map in getAllByContentProviderId)
                {
                    var serviceProvider = _aNQContentServiceManageDb.ServiceProvider.FirstOrDefault(a => a.ServiceProviderId == map.ServiceProviderId);

                    if (serviceProvider != null)
                        serviceProviders.Add(serviceProvider);
                }

                var result = serviceProviders.Distinct(new SpComparer());

                return result.Count();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }

        }

        public async Task<MappedContentDto> GetAllMappedContentByMapContentToServiceProviderId(long mapContentToServiceProviderId)
        {
            try
            {
                var mappedContent = await _aNQContentServiceManageDb.MapContentToServiceProvider.FirstOrDefaultAsync(x => x.MapContentToServiceProviderId == mapContentToServiceProviderId);

                if (mappedContent != null)
                {
                    var contentDto = new MappedContentDto
                    {
                        MapContentToServiceProviderId = mappedContent.MapContentToServiceProviderId,
                        ContentProviderId = mappedContent.ContentProviderId,
                        ContentProviderName = mappedContent.ContentProviderName,
                        ContentId = mappedContent.ContentId,
                        ServiceProviderId = mappedContent.ServiceProviderId,
                        CreatedOn = mappedContent.CreatedOn,
                        MappedBy = mappedContent.MappedBy,
                        RateId = mappedContent.RateId,
                        RateName = mappedContent.RateName,
                        DedicatedRateId = mappedContent.DedicatedRateId,
                        RateCurrency = mappedContent.RateCurrency,
                        IsPublished = mappedContent.IsPublished
                    };
                    return contentDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public async Task<int?> GetServicesMappedToContentByContentId(long contentId)
        {
            var mappedContent = await _aNQContentServiceManageDb.MapContentToServiceProvider.Where(r => r.ContentId == contentId).ToListAsync();
            return mappedContent?.Count();
        }

        public async Task SaveAsync()
        {
            await _aNQContentServiceManageDb.SaveChangesAsync();
        }

        #region Checks

        public async Task<bool> HasContentBeenMapped(long contentId, long serviceProviderId)
        {
            try
            {
                if (contentId > 0 && serviceProviderId > 0)
                {
                    var checkIfContentExist = await _aNQContentServiceManageDb.MapContentToServiceProvider
                        .FirstOrDefaultAsync(x => x.ContentId.Equals(contentId) && x.ServiceProviderId.Equals(serviceProviderId));
                    if (checkIfContentExist != null)
                    {
                        return true;
                    }
                    return false;
                }

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public async Task<IEnumerable<ServiceProviderDto>> GetDistinctSpforCpByContentProviderId(long id)
        {

            List<ServiceProvider> serviceProviders = new List<ServiceProvider>();
            var getAllByContentProviderId = await _aNQContentServiceManageDb.MapContentToServiceProvider.
                Where(x => x.ContentProviderId == id).OrderByDescending(a => a.CreatedOn).ToListAsync();

            foreach (var map in getAllByContentProviderId)
            {
                var serviceProvider = _aNQContentServiceManageDb.ServiceProvider.FirstOrDefault(a => a.ServiceProviderId == map.ServiceProviderId);

                if (serviceProvider != null)
                {
                    if (!serviceProviders.Any(r => r.ServiceProviderId == serviceProvider.ServiceProviderId))
                    {
                        serviceProviders.Add(serviceProvider);
                    }
                    
                }
                    
            }
            return serviceProviders.Select(y => new ServiceProviderDto
            {
                AccountEmail = y.AccountEmail,
                ServiceProviderId = y.ServiceProviderId,
                ContactPersonEmail = y.ContactPersonEmail,
                ContactPersonFirstName = y.ContactPersonFirstName,
                ContactPersonLastName = y.ContactPersonLastName,
                ContactPersonPhoneNumber = y.ContactPersonPhoneNumber,
                ServiceProviderName = y.ServiceProviderName
                
            }).ToList();
        }

        public async Task<IEnumerable<ServiceProvider>> GetAllSPByCPIdAndContentId(long contentProviderId, long contentId)
        {

            List<ServiceProvider> serviceProviders = new List<ServiceProvider>();
            var getAllSPByCPIdAndContentId = await _aNQContentServiceManageDb.MapContentToServiceProvider.
                Where(x => x.ContentProviderId == contentProviderId && x.ContentId == contentId).OrderByDescending(a => a.CreatedOn).ToListAsync();

            foreach (var map in getAllSPByCPIdAndContentId)
            {
                var serviceProvider = _aNQContentServiceManageDb.ServiceProvider.FirstOrDefault(a => a.ServiceProviderId == map.ServiceProviderId);

                if (serviceProvider != null)
                    serviceProviders.Add(serviceProvider);
            }

            return serviceProviders;
        }


        public bool HasContentRateBeenMapped(long rateId)
        {
            var rate =  _aNQContentServiceManageDb.MapContentToServiceProvider.FirstOrDefault(ex => ex.RateId == rateId);
            if (rate != null)
            {
                return true;
            }
            return false;
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
                    // TODO: dispose managed state (managed objects).
                }

                disposedValue = true;
            }
        }


        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
            GC.CancelFullGCNotification();
        }

        #endregion
    }

    /// <summary>
    /// Compare helper method
    /// </summary>
    public class SpComparer : IEqualityComparer<ServiceProvider>
    {
        public bool Equals(ServiceProvider x, ServiceProvider y)
        {
            return x.ServiceProviderId == y.ServiceProviderId;
        }
       
        public int GetHashCode(ServiceProvider obj)
        {
            return obj.ServiceProviderId.GetHashCode();
        }
    }

}
