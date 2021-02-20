using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentServiceManagementAPI.HttpClientService;
using ContentServiceManagementAPI.Data;
using ContentServiceManagementAPI.Enums;
using ContentServiceManagementAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Infrastructure.Helpers;
using ContentProvider = ContentServiceManagementAPI.Models.ContentProvider;

namespace ContentServiceManagementAPI.Services.ContentProviderService
{
    public class ContentProviderAppService : IContentProviderAppService
    {
        private ANQContentServiceManageDb _aNQContentServiceManageDb;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ContentProviderAppService> _logger;
        private IAnqClientService _anqClientService;

        public ContentProviderAppService(ANQContentServiceManageDb aNQContentServiceManageDb, 
            IHttpContextAccessor httpContextAccessor, IAnqClientService anqClientService, ILogger<ContentProviderAppService> logger)
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            _httpContextAccessor = httpContextAccessor;
            _anqClientService = anqClientService;
            _logger = logger;
        }

        
        public async Task<ContentProvider> GetContentProviderIdByAuthId(long authId)
        {
            try
            {
                var cpAuthId = await _aNQContentServiceManageDb.ContentProvider.FirstOrDefaultAsync(x => x.AuthenticationId == authId);
                if(cpAuthId != null)
                {
                    var model = new ContentProvider
                    {
                        ContentProviderId = cpAuthId.ContentProviderId,
                        AuthenticationId = cpAuthId.AuthenticationId,
                        ContentProviderName = cpAuthId.ContentProviderName,
                        ContactPersonEmail = cpAuthId.ContactPersonEmail,
                        ContactPersonFirstName = cpAuthId.ContactPersonFirstName,
                        ContactPersonLastName = cpAuthId.ContactPersonLastName,
                        ContactPersonPhoneNumber = cpAuthId.ContactPersonPhoneNumber,
                        AccountEmail = cpAuthId.AccountEmail,
                        ApplicationId = cpAuthId.ApplicationId,
                        Status = cpAuthId.Status
                    };
                    return model;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        public async Task<ContentProviderDto> GetContentProviderByAuthId(long authId)
        {
                var contentProvider = await _aNQContentServiceManageDb.ContentProvider.FirstOrDefaultAsync(x => x.AuthenticationId == authId);
                if (contentProvider != null)
                {
                    ContentProviderDto item = new ContentProviderDto
                    {
                        AuthenticationId = contentProvider.AuthenticationId,
                        AccountEmail = contentProvider.AccountEmail,
                        ContentProviderName = contentProvider.ContentProviderName,
                        ContactPersonEmail = contentProvider.ContactPersonEmail,
                        ContactPersonFirstName = contentProvider.ContactPersonFirstName,
                        ContactPersonLastName = contentProvider.ContactPersonLastName,
                        ContactPersonPhoneNumber = contentProvider.ContactPersonPhoneNumber,
                        ContentProviderId = contentProvider.ContentProviderId,
                    };
                    return item;
                }
                else
                {
                    return null;
                }
        }

        public async Task<ContentProviderDto> GetContentProvider(long contentProviderId)
        {
            try
            {
                var contentProvider = await _aNQContentServiceManageDb.ContentProvider.FirstOrDefaultAsync(x => x.ContentProviderId == contentProviderId);   
                if(contentProvider != null)
                {
                    ContentProviderDto item = new ContentProviderDto
                    {
                        AuthenticationId = contentProvider.AuthenticationId,
                        AccountEmail = contentProvider.AccountEmail,
                        ContentProviderName = contentProvider.ContentProviderName,
                        ContactPersonEmail = contentProvider.ContactPersonEmail,
                        ContactPersonFirstName = contentProvider.ContactPersonFirstName,
                        ContactPersonLastName = contentProvider.ContactPersonLastName,
                        ContactPersonPhoneNumber = contentProvider.ContactPersonPhoneNumber,
                        ContentProviderId = contentProvider.ContentProviderId,               
                    };
                    return item;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message,e);
                return null;
            }
        }

        public async Task<IEnumerable<ContentProviderDto>> GetContentProviders()
        {
            try
            {
                var contentProvidersDto = new List<ContentProviderDto>();


                var model = await _aNQContentServiceManageDb.ContentProvider.OrderByDescending(or => or.ContentProviderId).ToListAsync();

                contentProvidersDto.AddRange(model.Select(x => new ContentProviderDto
                {
                    AuthenticationId = x.AuthenticationId,
                    AccountEmail = x.AccountEmail,
                    ContentProviderName = x.ContentProviderName,
                    ContactPersonEmail = x.ContactPersonEmail,
                    ContactPersonFirstName = x.ContactPersonFirstName,
                    ContactPersonLastName = x.ContactPersonLastName,
                    ContactPersonPhoneNumber = x.ContactPersonPhoneNumber,
                    ContentProviderId = x.ContentProviderId
                }));

                return contentProvidersDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }
           
        [Obsolete("We are no longer getting Content Providers from the Old ANQ rather users with Content Provider Roles.")]
        public async Task<ContentProvider> AddContentProvider(ContentProvider item)
        {    
            try
            {
                var contentProviders = await _aNQContentServiceManageDb.ContentProvider.Where(x => x.ContactPersonEmail.Equals(item.ContactPersonEmail) && (x.ContentProviderName.Equals(item.ContentProviderName) || x.AuthenticationId.Equals(item.AuthenticationId))).ToListAsync();
                if (contentProviders.Count() < 1)
                {
                    await _aNQContentServiceManageDb.ContentProvider.AddAsync(item);
                    await _aNQContentServiceManageDb.SaveChangesAsync();
                    _logger.LogInformation($"User has been added as a Content Provider");
                    return item;
                }
                return null;
            }
               
            catch(Exception e)
            {
                _logger.LogError(e.Message,e);
                return null;
            }
        }


        public EnumResult GetCurrency(int id)
        {
            var currency = EnumHelper.GetEnumResultByEnumId<CurrencyEnum>(id);
            if(currency != null)
            {
                return currency;
            }
            return null;
        }

        public async Task UpdateCommandRecord(int clientId)
        {
            try
            {
                //Get data from ANQ Old use web service
                _logger.LogInformation($"Fetching all content Providers from the old ANQ");
                var commandRecords = await _anqClientService.GetAnqCommandRecords(clientId);

                //get all avalailable CommandRecords
                var availeableCommandRecords = _aNQContentServiceManageDb.CommandRecords.Where(x => x.ClientId == clientId);

                // Compare both tables
                var contentProvidersToAdd = commandRecords.Where(c => !availeableCommandRecords.Any(a => c.CommandRecordId == a.CommandRecordId));

                if (contentProvidersToAdd.Count() > 0)
                {
                    foreach (var item in contentProvidersToAdd)
                    {
                        _logger.LogInformation($"Copying Command Records {item.SystemName} to ANQ Advanced.");
                        var data = new CommandRecord
                        {
                           ClientId = clientId,
                           CommandRecordId = item.CommandRecordId,
                           
                           Description = item.Description,
                           Group = item.Group,
                           SystemName = item.SystemName,
                           Type = item.Type

                        };
                        _aNQContentServiceManageDb.Database.ExecuteSqlCommand("SET Identity_Insert [dbo].[CommandRecords] ON");
                        await _aNQContentServiceManageDb.CommandRecords.AddAsync(data);
                        
                    }

                   
                    await _aNQContentServiceManageDb.SaveChangesAsync();
                    _aNQContentServiceManageDb.Database.ExecuteSqlCommand("SET Identity_Insert [dbo].[CommandRecords] OFF");
                    _logger.LogInformation($"Copying Command Records to ANQ Advanced completed successfully.");
                }
                else
                {
                    _logger.LogInformation($"Content Provider on ANQ Advanced is Updated.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
            }
        }

        public async Task<List<CommandRecord>> GetCommandRecordsByClientId(int clientId)
        {
            try
            {
                var commandRecords = _aNQContentServiceManageDb.CommandRecords.Where(x => x.ClientId == clientId);

                return await commandRecords.ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
            
        }
    }
}
