using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentServiceManagementAPI.HttpClientService;
using ContentServiceManagementAPI.Data;
using ContentServiceManagementAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Anq.Enums;
using ANQ.Notification.Services;
using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.ViewModels;
using ContentServiceManagementAPI.Models.DTO.ServiceProvider;
using ContentServiceManagementAPI.Helpers;

namespace ContentServiceManagementAPI.Services.ServiceProviderService
{
    public class ServiceProviderAppService : IServiceProviderAppService
    {
        private ANQContentServiceManageDb _aNQContentServiceManageDb;
        private IHttpContextAccessor _httpContextAccessor;
        private ILogger<ServiceProviderAppService> _logger;
        private readonly IProcessNotification _processNotification;
        private IAnqClientService _anqClientService;

        public ServiceProviderAppService(
            ANQContentServiceManageDb aNQContentServiceManageDb,
            IHttpContextAccessor httpContextAccessor, 
            IAnqClientService anqClientService, 
            IProcessNotification processNotification, 
            ILogger<ServiceProviderAppService> logger)
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            _httpContextAccessor = httpContextAccessor;
            _anqClientService = anqClientService;
            _logger = logger;
            _processNotification = processNotification;
        }
        public async Task<ServiceProviderDto> GetServiceProvider(long serviceProviderId)
        {
            try
            {
                _logger.LogInformation($"The Serilog service is running at Service Provider Service level");
                var serviceProvider = await _aNQContentServiceManageDb.ServiceProvider.FirstOrDefaultAsync(x => x.ServiceProviderId == serviceProviderId);

                if (serviceProvider != null)
                {
                    ServiceProviderDto item = new ServiceProviderDto
                    {
                        AuthenticationId = serviceProvider.AuthenticationId,
                        Status = serviceProvider.Status,
                        ContactPersonEmail = serviceProvider.ContactPersonEmail,
                        ContactPersonFirstName = serviceProvider.ContactPersonFirstName,
                        ContactPersonLastName = serviceProvider.ContactPersonLastName,
                        ContactPersonPhoneNumber = serviceProvider.ContactPersonPhoneNumber,
                        ServiceProviderId = serviceProvider.ServiceProviderId,
                        AccountEmail = serviceProvider.AccountEmail,
                        ServiceProviderName = serviceProvider.ServiceProviderName,
                        Approved = serviceProvider.Approved,
                        VasLicenseId = serviceProvider.VasLicenseId,
                        VasLicenseActiveDate = serviceProvider.VasLicenseActiveDate,
                        VasLicenseExpiryDate = serviceProvider.VasLicenseExpiryDate

                    };
                    return item;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
             _logger.LogError(e.Message,e);
                throw e;
            }

        }

        public async Task<IEnumerable<ServiceProviderDto>> GetServiceProviders()
        {
            try
            {
                var serviceProvidersDto = new List<ServiceProviderDto>();

                var model = await _aNQContentServiceManageDb.ServiceProvider.OrderByDescending(or => or.ServiceProviderId).ToListAsync();

                serviceProvidersDto.AddRange(model.Select(x => new ServiceProviderDto
                {
                    AuthenticationId = x.AuthenticationId,
                    Status = x.Status,
                    ContactPersonEmail = x.ContactPersonEmail,
                    ContactPersonFirstName = x.ContactPersonFirstName,
                    ContactPersonLastName = x.ContactPersonLastName,
                    ContactPersonPhoneNumber = x.ContactPersonPhoneNumber,
                    ServiceProviderName = x.ServiceProviderName,
                    ServiceProviderId = x.ServiceProviderId,
                    AccountEmail = x.AccountEmail,
                    ApplicationId = x.ApplicationId,
                    Approved = x.Approved,
                    VasLicenseId = x.VasLicenseId,
                    VasLicenseActiveDate = x.VasLicenseActiveDate,
                    VasLicenseExpiryDate = x.VasLicenseExpiryDate
                }));

                return serviceProvidersDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
                   
        }
           

        public async Task<ServiceProvider> AddServiceProvider(ServiceProvider model)
        {
            try
            {

                var contentProviders = await _aNQContentServiceManageDb.ServiceProvider.Where(x => x.ContactPersonEmail.Equals(model.ContactPersonEmail) && (x.ServiceProviderName.Equals(model.ServiceProviderName) || x.AuthenticationId.Equals(model.AuthenticationId))).ToListAsync();
                if (!contentProviders.Any())
                {
                    await _aNQContentServiceManageDb.ServiceProvider.AddAsync(model);
                    await _aNQContentServiceManageDb.SaveChangesAsync();
                    _logger.LogInformation($"User has been added as a Service Provider");
                    return model;
                }
                return model;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                return null;
            }
        }

        public async Task<ServiceProviderDto> GetServiceProviderByAuthId(long authId)
        {
            try
            {

                var data = _aNQContentServiceManageDb.ServiceProvider.Where(x => x.AuthenticationId == authId);
                var serviceProvider = data.FirstOrDefault();

                if (serviceProvider != null)
                {
                    ServiceProviderDto item = new ServiceProviderDto
                    {
                        AuthenticationId = serviceProvider.AuthenticationId,
                        Status = serviceProvider.Status,
                        ContactPersonEmail = serviceProvider.ContactPersonEmail,
                        ContactPersonFirstName = serviceProvider.ContactPersonFirstName,
                        ContactPersonLastName = serviceProvider.ContactPersonLastName,
                        ContactPersonPhoneNumber = serviceProvider.ContactPersonPhoneNumber,
                        ServiceProviderId = serviceProvider.ServiceProviderId,
                        AccountEmail = serviceProvider.AccountEmail,
                        ServiceProviderName = serviceProvider.ServiceProviderName,
                        Approved = serviceProvider.Approved,
                        CountryCode=serviceProvider.CountryCode,

                        CountryName=serviceProvider.CountryName,
                        AddressLocation=serviceProvider.AddressLocation

                    };
                    return item;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }

        }

        public async Task UpdateApprovedStatus(ServiceProviderDto serviceProviderDto)
        {
            try
            {
                var ServiceProvider = _aNQContentServiceManageDb.ServiceProvider.FirstOrDefault(a => a.ServiceProviderId == serviceProviderDto.ServiceProviderId);
                ServiceProvider.Approved = serviceProviderDto.Approved;
                _aNQContentServiceManageDb.ServiceProvider.Update(ServiceProvider);
                _aNQContentServiceManageDb.SaveChanges();

                ApplicationEnum Application = (ApplicationEnum.DND);

                string NotificationTemplate = string.Empty;
                string Subject = string.Empty;

                if (serviceProviderDto.Approved == "A")
                {
                    NotificationTemplate = NotificationTemplates.ApproveSPMessage;
                    Subject = "Account Verification Notification";
                }
                if (serviceProviderDto.Approved != "A")
                {
                    NotificationTemplate = NotificationTemplates.DeclineSPMessage;
                    Subject = "Account Verification Notification";
                }

                NotificationTemplate = NotificationTemplate
                    .Replace("{message}", $"{serviceProviderDto.DeclineReason}")
                    .Replace("{ServiceProviderName}", $"{ServiceProvider.ServiceProviderName}");

                await Task.Run(() =>
                {
                    _processNotification.ProcessNotificationAsync(Subject, NotificationTemplate, ServiceProvider.AccountEmail, Application);
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
            
        }

        public async Task UpdateDndServiceProvider(DndSpUpdateRequest dndSpUpdateRequest)
        {
            try
            {
                var ServiceProvider = _aNQContentServiceManageDb.ServiceProvider.FirstOrDefault(a => a.ServiceProviderId == dndSpUpdateRequest.ServiceProviderId);
                if(ServiceProvider != null)
                {
                    ServiceProvider.ContactPersonFirstName = dndSpUpdateRequest.FirstName;
                    ServiceProvider.ContactPersonLastName = dndSpUpdateRequest.LastName;
                    ServiceProvider.ContactPersonPhoneNumber = dndSpUpdateRequest.PhoneNumber;

                    _aNQContentServiceManageDb.ServiceProvider.Update(ServiceProvider);
                    _aNQContentServiceManageDb.SaveChanges();
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }
    }
}
