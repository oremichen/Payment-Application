using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Anq.Enums;
using Anq.ErrorMessages;
using ANQ.Messages.NonScheduledTasks.ContentService.Models;
using ANQ.Messages.NonScheduledTasks.Rates;
using ContentServiceManagementAPI.ContentServiceLogger;
using ContentServiceManagementAPI.Events.Publish.ServiceEvents;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO;
using ContentServiceManagementAPI.Models.DTO.ServiceContentMapping;
using ContentServiceManagementAPI.Services;
using ContentServiceManagementAPI.Services.ContentProviderService;
using ContentServiceManagementAPI.Services.ServiceContentMapService;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Logging;

namespace ContentServiceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceContentMapController : ControllerBase
    {
        #region Fields
        private IServiceContentMapAppService _serviceContentMappAppService;
        private IContentServiceEvent _contentServiceEvent;
        private IContentAppService _contentAppService;
        private readonly ILogger<ServiceContentMapController> _logger;
        private IContentProviderAppService _contentProviderAppService;
        private String _userName;
        private readonly ICServiceLogger _cServiceLogger;
        #endregion

        #region Ctor
        public ServiceContentMapController(
            IServiceContentMapAppService serviceContentMappAppService, 
            ILogger<ServiceContentMapController> loggerFactory,
            IContentAppService contentAppService, 
            IContentProviderAppService contentProviderAppService,
            IContentServiceEvent contentServiceEvent, 
            ICServiceLogger cServiceLogger)
        {
            _serviceContentMappAppService = serviceContentMappAppService;
            _contentAppService = contentAppService;
            _logger = loggerFactory;
            _userName = "Someone";
            _contentProviderAppService = contentProviderAppService;
            _contentServiceEvent = contentServiceEvent;
            _cServiceLogger = cServiceLogger;
        }
        #endregion
         
        #region Action Method

        /// <summary>
        /// This method includes functionality to publish Content service provider
        /// map details to billing content and content processing database
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateMap([FromBody] ServiceContentRateMapDto models)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkCount = 0;
                    var mapContentToServiceDtos = new List<MapContentToServiceDto>();

                    foreach (var model in models.ServiceContentMap)
                    {
                        bool checkName = await _serviceContentMappAppService.CheckIfContentBeenMappedToService(model.ServiceId, model.ContentId);
                        if (checkName == false)
                        {
                            await _serviceContentMappAppService.AddSync(model);
                            var content = await _contentAppService.GetById(model.ContentId);
                            var contentProvider = await _contentProviderAppService.GetContentProvider(content.ContentProviderId);

                            mapContentToServiceDtos.Add(new MapContentToServiceDto
                            {
                                ContentProviderName = contentProvider.ContentProviderName ?? null,
                                ContentProviderId = contentProvider.ContentProviderId,
                                Balance = 0,
                                ContentId = model.ContentId,
                                ContentName = content.Name,
                                ServiceId = model.ServiceId,
                                Status = Anq.Enums.EntityStatus.InsufficientBalance,
                                ContentProviderEmail = contentProvider.AccountEmail
                            });
                        }
                        else
                        {
                            checkCount++;
                        }
                    }
                    if (checkCount == models.ServiceContentMap.Count())
                    {
                        return BadRequest("A service already exist with this collection of contents");
                    }
                    await _serviceContentMappAppService.SaveAsync();

                    foreach (var rateItem in models.ServiceRate)
                    {
                        foreach (var item in mapContentToServiceDtos)
                        {
                            //publishes to content processing
                            var contentProvider = await _contentProviderAppService.GetContentProvider(item.ContentProviderId);
                            await _contentServiceEvent.MapContentToServiceRequest(item);

                            foreach (var model in models.ServiceContentMap)
                            {
                                ContentToServiceProviderModelDto contentToServiceProvider = new ContentToServiceProviderModelDto
                                {                                   
                                    ContentProviderId = (int)item.ContentProviderId,
                                    ContentProviderName = item.ContentProviderName,
                                    ContentId = item.ContentId,
                                    ServiceProviderId = model.ServiceProviderId,
                                    CreatedOn = model.CreatedOn,
                                    RateId = rateItem.ServiceRateId,
                                    RateCurrency = rateItem.Currency,
                                    RateName = rateItem.Name
                                };
                                //publishes to content billing
                                await _contentServiceEvent.SendContentServiceProvider(contentToServiceProvider);
                            }
                        }
                    }           
                    return Ok();
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, exception.Message);

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateServiceContentMapDto[] models)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var checkCount = 0;
                    var mapContentToServiceDtos = new List<MapContentToServiceDto>();

                    foreach (var model in models)
                    {
                        bool checkName = await _serviceContentMappAppService.CheckIfContentBeenMappedToService(model.ServiceId, model.ContentId);
                        if (checkName == false)
                        {
                            await _serviceContentMappAppService.AddSync(model);
                            var content = await _contentAppService.GetById(model.ContentId);
                            var contentProvider = await _contentProviderAppService.GetContentProvider(content.ContentProviderId);

                            mapContentToServiceDtos.Add(new MapContentToServiceDto
                            {
                                ContentProviderName = contentProvider.ContentProviderName ?? null,
                                ContentProviderId = contentProvider.ContentProviderId,
                                Balance = 0,
                                ContentId = model.ContentId,
                                ContentName = content.Name,
                                ServiceId = model.ServiceId,
                                Status = Anq.Enums.EntityStatus.InsufficientBalance,
                                ContentProviderEmail = contentProvider.AccountEmail
                            });
                        }
                        else
                        {
                            checkCount++;
                        }
                    }
                    if (checkCount == models.Length)
                    {
                        return BadRequest("A service already exist with this collection of contents");
                    }
                    await _serviceContentMappAppService.SaveAsync();

                    foreach (var item in mapContentToServiceDtos)
                    {
                        var contentProvider = await _contentProviderAppService.GetContentProvider(item.ContentProviderId);
                        await _contentServiceEvent.MapContentToServiceRequest(item);
                    }
                    return Ok();
                }

                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, exception.Message);

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateRange([FromBody] EditServceContentMapByList model)
        {
            try
            {
                if (model.ServiceContentMaps.Count > 0)
                {
                    int updateCount = 0;
                    foreach (var item in model.ServiceContentMaps)
                    {
                        if (item != null)
                        {
                            var checkName = await _serviceContentMappAppService.CheckIfContentBeenMappedToService(item.ServiceId, item.ContentId);
                            if (checkName == false)
                            {
                                var serviceContentMap = new CreateServiceContentMapDto
                                {
                                    ServiceId = item.ServiceId,
                                    ContentId = item.ContentId,
                                    ServiceProviderId = item.ServiceProviderId,
                                    CreatedOn = DateTime.UtcNow,
                                };
                                await _serviceContentMappAppService.AddSync(serviceContentMap);
                                updateCount++;
                            }
                            else
                            {
                                var serviceContentMap = new ServiceContentMapp
                                {
                                    ServiceId = item.ServiceId,
                                    ContentId = item.ContentId,
                                    ServiceProviderId = item.ServiceProviderId,
                                    CreatedOn = DateTime.UtcNow,
                                    ServiceContentMappId = item.ServiceContentMapId
                                };
                                await _serviceContentMappAppService.Update(serviceContentMap);
                            }
                        }
                        continue;
                    }
                    return Ok(new { message = $"{updateCount} contents added, Update successfull" });
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught Update failed. serviceContentMapp List object : {model}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromBody] ServiceContentMapDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = await _serviceContentMappAppService.GetById(model.ServiceId);
                    var dataContent = await _contentAppService.GetById(model.ContentId);
                    var dataServiceMapDto = new ServiceContentMapp
                    {
                        ServiceContentMappId = data.ServiceContentMappId,
                        ServiceId = data.ServiceId,
                        ContentId = data.ContentId,
                        CreatedOn = data.CreatedOn,
                        ServiceProviderId = data.ServiceProviderId
                    };
                    if (data != null)
                    {
                        var checkName = await _serviceContentMappAppService.CheckIfContentBeenMappedToService(model.ServiceId, model.ContentId);
                        if (checkName == false)
                        {
                            await _serviceContentMappAppService.Update(dataServiceMapDto);

                            _logger.LogInformation($"MappedContentTo Service entity with id : {model.ServiceId} was succesffully updated. PerformedBy {_userName}");
                            return Ok("Enitity Successfully updated");
                        }
                        return BadRequest(ErrorMessage.ALREADY_EXIST);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught Update failed. ServiceId : {model.ServiceId }, contentId : {model.ContentId}. ActionBy : {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpDelete]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Remove(ServiceContentMapDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceToDelete = await _serviceContentMappAppService.GetById(model.ServiceId);
                    if (serviceToDelete != null)
                    {
                        await _serviceContentMappAppService.Delete(model);
                        _logger.LogInformation($"Service Content Mapping was deleted successfully. PerfromedBy {_userName}");
                        return Ok("Service Content Mapping was deleted successfully");
                    }

                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Service was not deleted. Exception was caught. PerfromedBy - {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);

            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ServiceContentMapDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var data = await _serviceContentMappAppService.GetById(id);
                if (data != null)
                {
                    var serviceDto = new ServiceContentMapDto()
                    {
                        ServiceId = data.ServiceId,
                        ServiceContentMapId = data.ServiceContentMappId,
                        ServiceProviderId = data.ServiceProviderId,
                        CreatedOn = data.CreatedOn,
                        ContentId = data.ContentId,
                    };

                    var getContent = await _contentAppService.GetById(serviceDto.ContentId);
                    if (getContent != null)
                    {
                        serviceDto.Content = getContent;
                        var contentProvider = await _contentProviderAppService.GetContentProvider(getContent.ContentProviderId);
                        serviceDto.ContentProviderName = contentProvider.ContentProviderName;
                    }
                    else
                    {
                        serviceDto.Content = null;
                        serviceDto.ContentProviderName = "";

                    }

                    serviceDto.Content = await _contentAppService.GetById(serviceDto.ContentId);

                    _logger.LogInformation($"Success! Service details retuned. ActionBy{_userName}");
                    return Ok(serviceDto);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception e)
            {
                //_logger.LogError(ex, $"Exception caught! Service detail not returned. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<ServiceContentMapDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var mappedContentsToServiceDtos = await _serviceContentMappAppService.GetAll();
                var servicesContentDto = new List<ServiceContentMapDto>();

                if (mappedContentsToServiceDtos != null || mappedContentsToServiceDtos.Count() > 0)
                {
                    servicesContentDto.AddRange(mappedContentsToServiceDtos.OrderBy(c => c.ServiceId).Select(x => new ServiceContentMapDto()
                    {
                        ServiceId = x.ServiceId,
                        ServiceProviderId = x.ServiceProviderId,
                        CreatedOn = x.CreatedOn,
                        ContentId = x.ContentId,
                        ServiceContentMapId = x.ServiceContentMappId,

                    }));
                    foreach (var item in servicesContentDto)
                    {
                        var getContent = await _contentAppService.GetById(item.ContentId);
                        if (getContent != null)
                        {
                            item.Content = getContent;
                            var contentProvider = await _contentProviderAppService.GetContentProvider(item.Content.ContentProviderId);
                            item.ContentProviderName = contentProvider.ContentProviderName;
                        }
                        else
                        {
                            item.Content = null;
                            item.ContentProviderName = "";
                        }

                    }
                    _logger.LogInformation($"Success! All mapped content to services list retuned. ActionBy{_userName}");
                    return Ok(servicesContentDto);
                }

                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception e)
            {
                // _logger.LogError(ex, $"Exception caught! mapped services to content not returned. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<ServiceContentMapDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByServiceId(long id)
        {
            try
            {
                var mappedContentsToServiceDtos = await _serviceContentMappAppService.GetAllByServiceId(id);
                var servicesContentDto = new List<ServiceContentMapDto>();
                if (mappedContentsToServiceDtos != null)
                {
                    servicesContentDto.AddRange(mappedContentsToServiceDtos.OrderBy(c => c.CreatedOn).Select(x => new ServiceContentMapDto()
                    {
                        ServiceId = x.ServiceId,
                        ServiceProviderId = x.ServiceProviderId,
                        CreatedOn = x.CreatedOn,
                        ServiceContentMapId = x.ServiceContentMappId,
                        ContentId = x.ContentId,
                    }));
                    foreach (var item in servicesContentDto)
                    {
                        var getContent = await _contentAppService.GetById(item.ContentId);
                        if (getContent != null)
                        {
                            item.Content = getContent;
                            var contentProvider = await _contentProviderAppService.GetContentProvider(item.Content.ContentProviderId);
                            if (contentProvider != null)
                            {
                                item.ContentProviderName = contentProvider.ContentProviderName;
                            }
                            else
                            {

                                item.ContentProviderName = "";
                            }
                        }
                        else
                        {
                            item.Content = null;
                            item.ContentProviderName = "";
                        }
                    }
                    _logger.LogInformation($"Success! All mapped content to a service list returned. ActionBy{_userName}");
                    return Ok(servicesContentDto);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception e)
            {
                //_logger.LogError(ex, $"Exception caught! mapped contents to service not returned. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<ServiceContentMapServiceNameDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByContentId(long id)
        {
            try
            {
                var mappedContentsToServiceDtos = await _serviceContentMappAppService.GetAllByContentId(id);

                if (mappedContentsToServiceDtos != null)
                {
                    _logger.LogInformation($"Success! All content mapped to service by contentId");
                    return Ok(mappedContentsToServiceDtos);
                }
               
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception e)
            {
                //_logger.LogError(ex, $"Exception caught! mapped contents to service not returned. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        #endregion

    }
}


