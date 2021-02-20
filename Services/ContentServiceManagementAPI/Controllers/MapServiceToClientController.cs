
using Anq.ErrorMessages;
using ANQ.Messages.NonScheduledTasks.ContentService.Models;
using ContentServiceManagementAPI.ContentServiceLogger;
using ContentServiceManagementAPI.Events.Publish.ServiceEvents;
using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DTO.Service;
using ContentServiceManagementAPI.Models.DTO.ServiceMappingDto;
using ContentServiceManagementAPI.Services;
using ContentServiceManagementAPI.Services.ClientAppService;
using ContentServiceManagementAPI.Services.NotificationService;
using ContentServiceManagementAPI.Services.ServiceAppService;
using ContentServiceManagementAPI.Services.ServiceProviderService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Anq.Helpers;
using ContentServiceManagementAPI.Models.DomainModels;
using ANQ.Messages.NonScheduledTasks.Rates;

namespace ContentServiceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapServiceToClientController : ControllerBase
    {
        #region Fields
        private string _userName;
        private readonly IMapServiceToClientAppService _mapServiceToClient;
        private readonly IClientAppService _clientAppService;
        private readonly IServiceAppService _serviceAppService;
        private readonly IServiceProviderAppService _serviceProviderAppService;
        private readonly IContentServiceEvent _contentServiceEvent;
        private readonly INotificationService _notificationService;
        private readonly ILogger<MapServiceToClientController> _logger;
        private readonly ICServiceLogger _cServiceLogger;


        #endregion

        #region ctr
        public MapServiceToClientController(IMapServiceToClientAppService mapServiceToClient,
                                            IClientAppService clientAppService,
                                            IServiceAppService serviceAppService,
                                            ILogger<MapServiceToClientController> logger,
                                            IServiceProviderAppService serviceProviderAppService,
                                            IContentServiceEvent contentServiceEvent,
                                            INotificationService notificationService,
                                            ICServiceLogger cServiceLogger)
        {
            _mapServiceToClient = mapServiceToClient;
            _serviceProviderAppService = serviceProviderAppService;
            _clientAppService = clientAppService;
            _serviceAppService = serviceAppService;
            _contentServiceEvent = contentServiceEvent;
            _logger = logger;
            _userName = "someone";
            _notificationService = notificationService;
            _cServiceLogger = cServiceLogger;
        }
        #endregion

        #region Action Methods
        /// <summary>
        ///  Map a service to a Client
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Map([FromBody] AddMapServiceDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool hasServiceBeenMapped = await _mapServiceToClient.HasServiceBeenMapped(model.ServiceId, model.ClientId);

                    if (hasServiceBeenMapped == false)
                    {
                        var mapServiceToClient = new MapServiceToClient
                        {
                            ServiceId = model.ServiceId,
                            ServiceProviderId = model.ServiceProviderId,
                            ClientId = model.ClientId,
                            ClientName = model.ClientName,
                            ClientAuthId = model.ClientAuthId,// to be verified
                            CreatedOn = model.CreatedOn,
                            MappedBy = model.MappedBy,
                            RateId = model.RateId,
                            RateName = model.RateName,
                            ServiceName = model.ServiceName,
                            ServiceProviderName = model.ServiceProviderName,
                            RateCurrency = (int)model.RateCurrency,
                        };

                        await _mapServiceToClient.Add(mapServiceToClient);

                        await _mapServiceToClient.SaveAsync();
                        _logger.LogInformation($"Success! Service mapped to client. ActionBy - {_userName}");

                        // publish to RabbitMq
                        await PublishMessage(mapServiceToClient);                     

                        //Send Notification to Service Provider
                        await _notificationService.SendServiceProvider_ServiceClientMapNotification(model);

                        //Send to Client
                        await _notificationService.SendClient_ServiceClientMapNotification(model);

                        return Ok(mapServiceToClient.MapServiceToClientId);
                    }
                    return BadRequest(ErrorMessage.CUSTOM_Already_Exist(model.ServiceName));
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught. Mapping failed. ActionBy {_userName}");
                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(String), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> MapByList([FromBody] AddMapServiceDto[] models)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int lengthOfrepeated = 0;
                    string msg = "";
                    var mappedData = new List<MapServiceToClient>();
                    foreach (var model in models)
                    {
                        bool hasServiceBeenMapped = await _mapServiceToClient.HasServiceBeenMapped(model.ServiceId, model.ClientId);
                        if (hasServiceBeenMapped == false)
                        {
                            var mapServiceToClient = new MapServiceToClient
                            {
                                ServiceId = model.ServiceId,
                                ServiceProviderId = model.ServiceProviderId,
                                ClientId = model.ClientId,
                                CreatedOn = model.CreatedOn,
                                MappedBy = model.MappedBy,
                                ServiceName = model.ServiceName,
                                RateId = model.RateId,
                                RateName = model.RateName,
                                ServiceProviderName = model.ServiceProviderName,
                                RateCurrency = (int)model.RateCurrency,
                                DedicatedRateId = model.DedicatedRateId,
                                DedicatedRateName = model.DedicatedRateName,
                            };
                            await _mapServiceToClient.Add(mapServiceToClient);

                            mappedData.Add(mapServiceToClient);
                        }
                        else
                        {
                            lengthOfrepeated = lengthOfrepeated + 1;
                            msg = $" NOTE! {lengthOfrepeated} service(s) has been mapped to you previously,therefore not mapped again";
                        }
                        continue;
                    }
                    await _mapServiceToClient.SaveAsync();
                    _logger.LogInformation($"Success! Service mapped to client. ActionBy - {_userName}");

                    // publish mapped data to RabbitMq
                    await PublishMessage(mappedData);

                    return Ok(new { message = $"Services Successfully Mapped To Client{msg}" });
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught. Mapping failed. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Edit Mapped Service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromBody] MappedServiceDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _mapServiceToClient.Update(model);
                    _logger.LogInformation($"Success! Mapped Service Updated. ActionBy - {_userName}");
                    return Ok(new { message = "Mapping Details Updated Successfully" });
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught. Update of mapping failed.ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="MapServiceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(MappedServiceDto))]
        public async Task<IActionResult> GetById([FromQuery]long id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedService = await _mapServiceToClient.GetMappedServiceById(id);
                    if (mappedService != null)
                    {
                        _logger.LogInformation($"Success! User Fetched Mapping Details by its id. ActionBy - {_userName}");
                        return Ok(mappedService);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught. Fetching of mapped service failed .ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Get all mapped services
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(IEnumerable<MappedServiceDto>))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var mappedservices = await _mapServiceToClient.GetAll();
                return Ok(mappedservices);

            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught. Fetching of all mapped services failed .ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Get all services to mapped to a client
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(IEnumerable<MappedServicesToAClientDto>))]
        public async Task<IActionResult> GetAllByClientId([FromQuery]int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappServicesForClient = await _mapServiceToClient.GetAllByClientId(id);
                    if (mappServicesForClient != null)
                    {
                        var data = new List<MappedServicesToAClientDto>();

                        data.AddRange(mappServicesForClient.OrderByDescending(x => x.CreatedOn).Select(x => new MappedServicesToAClientDto()
                        {
                            MappedServiceId = x.MappedServiceId,
                            ServiceId = x.ServiceId,
                            ClientId = x.ClientId,
                            ServiceProviderId = x.ServiceProviderId,
                            ServiceProviderName = x.ServiceProviderName,
                            CreatedOn = x.CreatedOn,
                            RateId = x.RateId,
                            RateName = x.RateName,
                            ServiceName = x.ServiceName,
                            MappedBy = x.MappedBy,
                            RateCurrency = (int)x.RateCurrency,
                            RateCurrencyDescription = EnumerationHelper.GetDescription(x.RateCurrency),
                            DedicatedRateId = x.DedicatedRateId,
                            DedicatedRateName = x.DedicatedRateName,
                        }));
                        foreach (var item in data)
                        {
                            var mappservice = await _serviceAppService.GetById(item.ServiceId);
                            if (mappservice != null)
                            {
                                var serviceDto = new ServiceDto()
                                {
                                    ServiceId = mappservice.ServiceId,
                                    ServiceName = mappservice.ServiceName,
                                    ServiceProviderId = mappservice.ServiceProviderId,
                                    CreatedOn = mappservice.CreatedOn,
                                    Note = mappservice.Note,
                                    ServiceProviderName = mappservice.ServiceProviderName,
                                    Active = mappservice.Active,

                                };
                                item.Service = serviceDto;
                            }
                            else
                            {
                                item.Service = null;
                            }
                        }
                        return Ok(data);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught. Fetching of all mapped services for a client failed .ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Get all services to mapped to a client
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(IEnumerable<MappedClientsToAServiceDto>))]
        public async Task<IActionResult> GetAllByServiceId([FromQuery]long id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedClientsForAService = await _mapServiceToClient.GetAllByServiceId(id);
                    if (mappedClientsForAService != null)
                    {
                        var mappClientsToServiceDto = new List<MappedClientsToAServiceDto>();

                        mappClientsToServiceDto.AddRange(mappedClientsForAService.OrderByDescending(x => x.CreatedOn).Select(x => new MappedClientsToAServiceDto()
                        {
                            MappedServiceId = x.MappedServiceId,
                            ServiceId = x.ServiceId,
                            ClientId = x.ClientId,
                            ServiceProviderId = x.ServiceProviderId,
                            CreatedOn = x.CreatedOn,
                            ServiceName = x.ServiceName,
                            RateId = x.RateId,
                            RateName = x.RateName,
                            MappedBy = x.MappedBy,
                            RateCurrency = (int)x.RateCurrency,
                            DedicatedRateId = x.DedicatedRateId,
                            DedicatedRateName = x.DedicatedRateName,
                        }));
                        _logger.LogInformation($"Success! User Fetched all mapped Service for a client. ActionBy - {_userName}");
                        foreach (var item in mappClientsToServiceDto)
                        {
                            var mappedClient = await _clientAppService.GetClientById(item.ClientId);
                            if (mappedClient != null)
                            {
                                item.Client = mappedClient;
                            }
                            else
                            {
                                item.Client = null;
                            }
                        }
                        return Ok(mappClientsToServiceDto);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught. Fetching of all mapped client to a service failed .ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        ///  Get all mapped service of a service provider
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(IEnumerable<MappedServiceDto>))]
        public async Task<IActionResult> GetAllByServiceProviderId([FromQuery]long id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedServicesForServiceProvider = await _mapServiceToClient.GetAllByServiceProviderId(id);
                    if (mappedServicesForServiceProvider != null)
                    {
                        _logger.LogInformation($"Success! User Fetched all mapped Service of a service Provider. ActionBy - {_userName}");
                        return Ok(mappedServicesForServiceProvider);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                // _logger.LogError(exception, $"Exception caught. Fetching of all mapped services for a serviceprovider failed .ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Delete a mapped service
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Remove([FromBody] MappedServiceDto model)
        {
            try
            {
                if (model.MappedServiceId > 0)
                {
                    await _mapServiceToClient.Delete(model);
                    _logger.LogInformation($"Success! Mapping deleted. -ActionBy {_userName}");

                    //publish to rabbitmq and to contentProcessing when unmapped
                    var unMapp = new MapServiceToClient
                    {
                        MapServiceToClientId = model.MappedServiceId,
                        ServiceId = model.ServiceId,
                        ServiceProviderId = model.ServiceProviderId,
                        ClientId = model.ClientId,
                        ClientName = model.ClientName,
                        ClientAuthId = model.ClientAuthId,// To be Verified
                        CreatedOn = model.CreatedOn,
                        MappedBy = model.MappedBy,
                        RateId = model.RateId,
                        RateName = model.RateName,
                        ServiceName = model.ServiceName,
                        ServiceProviderName = model.ServiceProviderName,
                        RateCurrency = (int)model.RateCurrency,
                    };
                    await UnMappedPublishMessage(unMapp);

                    return Ok(new { message = "Mapping Details Deleted Successfully" });
                }
                return BadRequest(ErrorMessage.NOT_FOUND);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught. Delete Failed to execute .ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        #endregion

        [HttpGet]
        [Route("[action]")]
        [Produces(typeof(ClientSkipTakeAndCountModelDto))]
        public async Task<IActionResult> GetAllClientsMappedToSPSercicesByServiceProviderId(long id,int skip, int take)
        {
            try
            {
                var getCommand = await _mapServiceToClient.GetAllMappedClientToSpServicesByserviceProviderId(id,skip, take);
                if (getCommand != null)
                {
                    return Ok(getCommand);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        [HttpGet]
        [Route("[action]")]
        [Produces(typeof(Client))]
        public async Task<IActionResult> GetAllMappedClientToSpServicesBySPIdDropdown(long id)
        {
            try
            {
                var getCommand = await _mapServiceToClient.GetAllMappedClientToSpServicesByServiceIdDropdown(id);
                if (getCommand != null)
                {
                    return Ok(getCommand);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        [HttpGet]
        [Route("[action]")]
        [Produces(typeof(ClientSkipTakeAndCountModelDto))]
        public async Task<IActionResult> GetAllMappedClientToSpServicesByserviceProviderIAndSearch(long id,string search = null)
        {
            try
            {


                var getCommand = await _mapServiceToClient.GetAllMappedClientToSpServicesByserviceProviderIAndSearch(id,search);
                if (getCommand != null)
                {
                    return Ok(getCommand);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        #region Helper
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task PublishMessage(MapServiceToClient model)
        {
            var data = new MapClientToserviceModelDto
            {
                Id = model.MapServiceToClientId,
                Balance = 0,
                ClientId = model.ClientId,
                ClientName = model.ClientName,
                ClientAuthId = model.ClientAuthId,//To be Verified
                ServiceId = model.ServiceId,
                Status = (int)Anq.Enums.EntityStatus.InsufficientBalance
            };
            // note: This request is consumed by content processing service

            await _contentServiceEvent.MapClientToServiceRequest(data);


            // note: This request is consumed by Billing subscriber
            var serviceClient = new ServiceClientMapDto
            {
                MapServiceToClientId = model.MapServiceToClientId,
                ClientAuthId = model.ClientAuthId,
                ClientId = model.ClientId,
                ServiceId = model.ServiceId,
                ServiceProviderId = model.ServiceProviderId,
                CreatedOn = model.CreatedOn,
                MappedBy = model.MappedBy,
                ServiceProviderName = model.ServiceProviderName,
                RateId = model.RateId,
                RateName = model.RateName,
                DedicatedRateId = model.DedicatedRateId,
                DedicatedRateName = model.DedicatedRateName,
                RateType = model.RateType,
                RateCurrency = model.RateCurrency,
                ServiceName = model.ServiceName,
                ClientName = model.ClientName
            };

            await _contentServiceEvent.SendServiceClient(serviceClient);


        }

        //unmapping of client/service
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task UnMappedPublishMessage(MapServiceToClient model)
        {
            var data = new MapClientToServiceDto
            {
                Id = model.MapServiceToClientId,
                Balance = 0,
                ClientId = model.ClientId,
                ClientName = model.ClientName,
                ServiceId = model.ServiceId,
                Status = Anq.Enums.EntityStatus.InsufficientBalance
            };
            await _contentServiceEvent.UnMapClientToServiceRequest(data);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task PublishMessage(List<MapServiceToClient> models)
        {
            foreach (var model in models)
            {
                var data = new MapClientToServiceDto
                {
                    Id = model.MapServiceToClientId,
                    Balance = 0,
                    ClientId = model.ClientId,
                    ClientName = model.ClientName,
                    ClientAuthId = model.ClientAuthId,//To be Verified
                    ServiceId = model.ServiceId,
                    Status = Anq.Enums.EntityStatus.InsufficientBalance
                };

                await _contentServiceEvent.MapClientToServiceRequest(data);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Produces(typeof(bool))]
        public IActionResult IsServiceRateMapped(long rateId)
        {
            try
            {
                var check = _mapServiceToClient.HasServiceRateBeenMapped(rateId);
                if (check == true)
                {
                    return Ok(true);
                }
                return Ok(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex;
            }
        }

        #endregion

    }
}
