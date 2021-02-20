using Anq.Enums;
using ANQ.Messages.NonScheduledTasks.ContentService.Models;
using ContentServiceManagementAPI.Configuration;
using ContentServiceManagementAPI.Events.Publish.ServiceEvents;
using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO.DeliveryService;
using ContentServiceManagementAPI.Models.DTO.Service;
using ContentServiceManagementAPI.Services.ClientAppService;
using ContentServiceManagementAPI.Services.ServiceAppService;
using ContentServiceManagementAPI.Services.VasSystemService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.VasProviderAppService
{
    public class VasProviderAppService : IVasProviderAppService
    {
        private readonly string AddSmsClientEndpoint = "api/smsclient/add";
        private readonly string GetServiceCurrencyRateMapsEndpoint = "api/ServiceRate/GetAllVhpDeliveryServiceRateMaps";
        private readonly IClientAppService _clientAppService;
        private readonly IServiceAppService _serviceAppService;
        private readonly VasHostingPlatformSettings vasHostingPlatformSettings;
        private readonly IMapServiceToClientAppService _mapServiceToClient;
        private readonly IContentServiceEvent _contentServiceEvent;
        private readonly IVasSystemServiceAppService _vasSystemServiceAppService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<VasProviderAppService> logger;

        public VasProviderAppService(
            IClientAppService clientAppService,
            IServiceAppService serviceAppService,
            IOptions<VasHostingPlatformSettings> options,
            IMapServiceToClientAppService mapServiceToClient,
            IContentServiceEvent contentServiceEvent,
            IVasSystemServiceAppService vasSystemServiceAppService,
            IHttpClientFactory httpClientFactory,
            ILogger<VasProviderAppService> logger
            )
        {
            _clientAppService = clientAppService;
            _serviceAppService = serviceAppService;
            vasHostingPlatformSettings = options.Value;
            _mapServiceToClient = mapServiceToClient;
            _contentServiceEvent = contentServiceEvent;
            _vasSystemServiceAppService = vasSystemServiceAppService;
            _httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task CompleteVasProviderConfiguration(VasProviderConfigurationDto vasProviderConfigurationDto)
        {
            try
            { 
                var serviceProviderDto = vasProviderConfigurationDto.serviceProviderDto;
                var serviceCurrencyRates = await GetServiceCurrencyRateMaps();

                //Create as Client
                #region Create VAS Provider as Client on ANQ
                Client client = new Client();
                client.AccountEmail = serviceProviderDto.AccountEmail;
                client.ApplicationId = ((long)ApplicationEnum.ANQ).ToString();
                client.AuthenticationId = serviceProviderDto.AuthenticationId;
                client.ClientName = serviceProviderDto.ServiceProviderName;
                client.ContactPersonEmail = serviceProviderDto.ContactPersonEmail;
                client.ContactPersonFirstName = serviceProviderDto.ContactPersonFirstName;
                client.ContactPersonLastName = serviceProviderDto.ContactPersonLastName;
                client.ContactPersonPhoneNumber = serviceProviderDto.ContactPersonPhoneNumber;
                client.Status = Enums.EntityStatus.Active;
                //client.CountryCode = serviceProviderDto.CountryCode;
                //client.CountryName = serviceProviderDto.CountryName;
                //client.AddressLocation = serviceProviderDto.AddressLocation;
              

                var model = await _clientAppService.AddClient(client);

                var _client = await _clientAppService.GetClientByAuthId(serviceProviderDto.AuthenticationId);
                model.ClientId = _client.ClientId;
                client.ClientId = _client.ClientId;
                #endregion


                //Map to Routing Service Nigeria, DND, 
                #region Map to Routing Service NG, DND
                foreach (var serviceName in vasHostingPlatformSettings.DeliveryServices)
                {
                    var service = await _serviceAppService.GetByServiceName(serviceName);

                    var serviceRateMap = serviceCurrencyRates.FirstOrDefault(a => a.ServiceName == serviceName);

                    if (service != null && serviceRateMap != null)
                    {
                        //Get the default rates


                        bool hasServiceBeenMapped = await _mapServiceToClient.HasServiceBeenMapped(service.ServiceId, model.ClientId);

                        if (hasServiceBeenMapped == false)
                        {
                            var mapServiceToClient = new MapServiceToClient
                            {
                                ServiceId = service.ServiceId,
                                ServiceProviderId = service.ServiceProviderId,
                                ClientId = model.ClientId,
                                ClientName = model.ClientName,
                                ClientAuthId = serviceProviderDto.AuthenticationId,// to be verified
                                CreatedOn = DateTime.Now,
                                MappedBy = string.Empty,
                                RateId = serviceRateMap.RateId,
                                RateName = serviceRateMap.RateName,
                                ServiceName = service.ServiceName,
                                ServiceProviderName = service.ServiceProviderName,
                                RateCurrency = serviceRateMap.RateCurrency,
                            };

                            await _mapServiceToClient.Add(mapServiceToClient);

                            await _mapServiceToClient.SaveAsync();

                            // publish to RabbitMq
                            var data = new MapClientToServiceDto
                            {
                                Id = 0,
                                Balance = 0,
                                ClientId = model.ClientId,
                                ClientName = model.ClientName,
                                ClientAuthId = serviceProviderDto.AuthenticationId,//To be Verified
                                ServiceId = service.ServiceId,
                                Status = Anq.Enums.EntityStatus.InsufficientBalance
                            };

                            await _contentServiceEvent.MapClientToServiceRequest(data);

                        }
                    }
                }
                #endregion


                //Create all System Services for User and send all to content processing
                #region Create System services for Service Provider
                var systemServices = await _vasSystemServiceAppService.GetAll();
                foreach (var _service in systemServices)
                {
                    Service service = new Service()
                    {
                        ServiceName = serviceProviderDto.ServiceProviderName + "'s " + _service.Name + " Service",
                        ServiceProviderId = serviceProviderDto.ServiceProviderId,
                        ServiceProviderName = serviceProviderDto.ServiceProviderName,
                        BillBasisId = 0,
                        BillPerTransactionCount = 0,
                        Category = 0,
                        Channel = "",
                        Frequency = 0,
                        Industry = 0,
                        Keyword = "",
                        MobileOriginatingCount = 0,
                        MobileTerminatingCount = 0,
                        OperatorIds = "",
                        Periodicity = 0,
                        Price = 0,
                        ServiceCode = "",
                        ServiceStatus = 0,
                        ShortCode = "",
                        SubscriptionType = 0,
                        ActiveDate = DateTime.Now,
                        ExpiryDate = DateTime.Now,
                        Active = true,
                        CreatedOn = DateTime.Now,
                        Note = string.Empty,
                        ApplicationId = (long)ApplicationEnum.DND,
                        IsVasSystemService = true,
                        VasSystemServiceCode = _service.Code
                    };

                    service.ServiceId = await _serviceAppService.Add(service);

                    bool add = await _serviceAppService.AddServiceOnContentProcessing(service);
                }
                #endregion


                //Send Details to DS
                #region Send the user details to DS
                SmsClient smsClient = new SmsClient();
                smsClient.AnqClientId = model.ClientId;
                smsClient.ApiKey = vasProviderConfigurationDto.ApiKey;
                smsClient.CreatorId = serviceProviderDto.ServiceProviderId.ToString();
                smsClient.DateCreated = DateTime.Now;
                smsClient.Email = serviceProviderDto.AccountEmail;
                smsClient.EntityStatus = 1;
                smsClient.FirstName = serviceProviderDto.ContactPersonFirstName;
                smsClient.LastName = serviceProviderDto.ContactPersonLastName;
                smsClient.Username = serviceProviderDto.AccountEmail;
                smsClient.CountryCode = serviceProviderDto.CountryCode;
                smsClient.CountryName = serviceProviderDto.CountryName;
                smsClient.AddressLocation = serviceProviderDto.AddressLocation;
                smsClient.AnqUserId = serviceProviderDto.AuthenticationId;

                await SendUserToDeliverySystem(smsClient);
                #endregion
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public async Task<List<ServiceCurrencyRateDto>> GetServiceCurrencyRateMaps()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("BillingContentService");
                var result = await client.GetAsync(GetServiceCurrencyRateMapsEndpoint);
                if (result.IsSuccessStatusCode)
                {
                    string response = await result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<ServiceCurrencyRateDto>>(response);

                    return data;
                }
            }
            catch(Exception e)
            {
                logger.LogError(e.Message, e);
                return new List<ServiceCurrencyRateDto>();
            }
            return new List<ServiceCurrencyRateDto>();
        }

        public async Task SendUserToDeliverySystem(SmsClient smsClient)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("CoureDeliverySmsClient");
                var result = await client.PostAsync(AddSmsClientEndpoint, new JsonContent(smsClient));
                if (result.IsSuccessStatusCode)
                {

                }
            }
            catch(Exception e)
            {
                logger.LogError(e.Message, e);
            }
        }
    }

    public class JsonContent : StringContent
    {
        public JsonContent(object obj) :
            base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        { }
    }
}