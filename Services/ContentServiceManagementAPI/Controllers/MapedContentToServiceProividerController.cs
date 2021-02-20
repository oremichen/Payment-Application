using Anq.ErrorMessages;
using ContentServiceManagementAPI.ContentServiceLogger;
using ContentServiceManagementAPI.Models.DTO.ContentMappingDto;
using ContentServiceManagementAPI.Services;
using ContentServiceManagementAPI.Services.NotificationService;
using ContentServiceManagementAPI.Services.ServiceContentMapService;
using ContentServiceManagementAPI.Services.ServiceProviderService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MapContentToServiceProividerController :ControllerBase
    {
        private  string _userName ;
        private readonly IMapContentToServiceProviderAppService _mapContentAppService;
        private readonly IContentAppService _contentAppService;
        private readonly ILogger<MapContentToServiceProividerController> _logger;
        private readonly IServiceProviderAppService _serviceProviderAppService;
        private readonly INotificationService _notificationService;
        private readonly ICServiceLogger _cServiceLogger;

        #region ctr
        public MapContentToServiceProividerController(IMapContentToServiceProviderAppService mapContentAppService, 
            ILogger<MapContentToServiceProividerController> loggerFactory,
            IContentAppService contentAppService, 
            IServiceProviderAppService serviceProviderAppService,
            INotificationService notificationService, ICServiceLogger cServiceLogger
            )
        {
            _mapContentAppService = mapContentAppService;
            _contentAppService = contentAppService;
            _logger = loggerFactory;
            _userName = "someone";
            _serviceProviderAppService = serviceProviderAppService;
            _notificationService = notificationService;
            _cServiceLogger = cServiceLogger;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Map a content to a service provider
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Map([FromBody] AddMapContentDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool hasContentBeenMapped = await _mapContentAppService.HasContentBeenMapped(model.ContentId, model.ServiceProviderId);
                    if (hasContentBeenMapped == false)
                    {
                        await _mapContentAppService.Add(model);

                        //Send Email to Content Provider
                        await _notificationService.SendContentProvider_ContentServiceProviderMapNotification(model);

                        //Send Email to Service Provider
                        await _notificationService.SendServiceProvider_ContentServiceProviderMapNotification(model);

                        _logger.LogInformation($"Content was succesffully Mapped to serviceprovider. PerformedBy {_userName}");
                        return Ok(new { message = "Content successfully mapped to serviceProvider" });
                    }
                    return BadRequest(ErrorMessage.ALREADY_EXIST);
                }
                return BadRequest(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Exception cuaght while trying to Map content to service provider");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Edit Mapped Content
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromBody] MappedContentDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _mapContentAppService.Update(model);
                    _logger.LogInformation($"Content successfully mapped to service provider. PerfomredBy {_userName}");
                    return Ok(new { mesage = "Mapping Details Updated Successfully" });
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Exception caught Update Failed, mappingId={model.MapContentToServiceProviderId} , PerformedBy{_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Delete a mapped content
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Remove([FromBody] MappedContentDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.MapContentToServiceProviderId >= 0)
                    {
                        await _mapContentAppService.Delete(model);
                        _logger.LogInformation($"Success, Mapping with id={model.MapContentToServiceProviderId} ha been Deleted. -ActionBy {_userName}");
                        return Ok(new { message = "Mapping Details Deleted Successfully" });
                    }
                    return BadRequest(ErrorMessage.NOT_FOUND);
                }

                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Exception caught. Delete failed. ActionBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        /// <summary>
        /// Get mapped content by table identifier
        /// </summary>
        /// <param name="mappedContentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        public async Task<IActionResult> GetById([FromQuery]long mappedContentId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var mappedContentDetail = await _mapContentAppService.GetById(mappedContentId);
                    if (mappedContentDetail != null)
                    {
                        _logger.LogInformation($"Success! Mapping fetched.ActionBy {_userName}");
                        return Ok(mappedContentDetail);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception ex)
            {
                //_logger.LogInformation(exception, $"Exception caught. Action to fetch Mapping failed. ActionBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Get all mapped contents
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]/mappcontent")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(IEnumerable<MappedContentDto>))]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var mappedContents = await _mapContentAppService.GetAll();
                foreach (var item in mappedContents)
                {
                    item.Content = await _contentAppService.GetById(item.ContentId);
                }
                return Ok(mappedContents);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Exception caught. Action to fetch Mapping failed. ActionBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// All contents not mapped to a service but mapped to a serviceprovider. Parameters include i.d of the service and id of the service provider
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="serviceProviderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<MappedContentDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllNotMappedToThisServiceByServiceProviderId([FromServices]IServiceContentMapAppService _serviceContentMapService, long serviceId, long serviceProviderId)
        {
            try
            {
                var contentsMapped = await _serviceContentMapService.GetAllByServiceId(serviceId);
                var mappedContentsToServiceProvider = await _mapContentAppService.GetMapedContentsForServiceProvider(serviceProviderId);

                var nonmappedContentsToServiceOfServiceProvider = mappedContentsToServiceProvider.Where(m => !contentsMapped.Any(c => c.ContentId == m.ContentId)).ToList();

                var servicesContentDto = new List<MappedContentDto>();
                if (nonmappedContentsToServiceOfServiceProvider != null)
                {
                    foreach (var item in nonmappedContentsToServiceOfServiceProvider)
                    {
                        item.Content = await _contentAppService.GetById(item.ContentId);
                    }
                    return Ok(nonmappedContentsToServiceOfServiceProvider);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Exception caught. Action to fetch a service provider mapped contents failed. ActionBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);

            }

        }

        /// <summary>
        /// Get all contents mapped to a service provider
        /// </summary>
        /// <param name="serviceProviderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(IEnumerable<MappedContentDto>))]
        public async Task<IActionResult> GetAllByServiceProviderId([FromQuery] long id)
        {
            try
            {
                var mappedContents = await _mapContentAppService.GetMapedContentsForServiceProvider(id);
                if (mappedContents != null)
                {
                    foreach (var item in mappedContents)
                    {
                        item.Content = await _contentAppService.GetById(item.ContentId);
                    }

                    _logger.LogInformation($"Success! All mapped contents for service Provider with id {id} fetched from db.actionBy -{_userName}");

                    return Ok(mappedContents);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Exception caught. Action to fetch a service provider mapped contents failed. ActionBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Get all mapped content for a content provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(IEnumerable<MappedContentDto>))]
        public async Task<IActionResult> GetAllByContentProviderId([FromQuery]int id)
        {
            try
            {
                var mappedContents = await _mapContentAppService.GetMappedContentsForContentProvider(id);
                if (mappedContents != null)
                {
                    foreach (var item in mappedContents)
                    {
                        var mappedContent = await _contentAppService.GetById(item.ContentId);
                        if (mappedContent != null)
                        {
                            item.Content = mappedContent;
                        }
                        else
                        {
                            item.Content = null;
                        }
                        var mappedServiceProviders = await _serviceProviderAppService.GetServiceProvider(item.ServiceProviderId);
                        if (mappedServiceProviders != null)
                        {
                            item.ServiceProvider = mappedServiceProviders;
                        }
                        else
                        {
                            item.ServiceProvider = null;
                        }
                    }

                    _logger.LogInformation($"Success! All mapped contents for content Provider with id {id} fetched from db. actionBy {_userName}");
                    return Ok(mappedContents);

                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Exception caught. Action to fetch a content provider mapped contents failed. ActionBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        /// <summary>
        /// Get all mapped content for a content provider
        /// </summary>
        /// <param name="contentProviderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(IEnumerable<MappedContentDto>))]
        public async Task<IActionResult> GetAllByContentId([FromQuery]long id)
        {
            try
            {
                var mappedContents = await _mapContentAppService.GetAllByContentId(id);
                if (mappedContents != null)
                {
                    foreach (var item in mappedContents)
                    {
                        var mappedContent = await _contentAppService.GetById(item.ContentId);
                        if (mappedContent != null)
                        {
                            item.Content = mappedContent;
                        }
                        else
                        {
                            item.Content = null;
                        }
                        var mappedServiceProviders = await _serviceProviderAppService.GetServiceProvider(item.ServiceProviderId);
                        if (mappedServiceProviders != null)
                        {
                            item.ServiceProvider = mappedServiceProviders;
                        }
                        else
                        {
                            item.ServiceProvider = null;
                        }
                    }

                    _logger.LogInformation($"Success! All mapped service Providers with content id {id} fetched from db. actionBy {_userName}");
                    return Ok(mappedContents);

                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Exception caught. Action to fetch a service provider mapped contents failed. ActionBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpGet]   
        [Route("[action]")]
        public async Task<IActionResult> GetAllSpMappedToCpContentByContentProviderId([FromQuery]long id)
        {
            try
            {
                var getCommand = await _mapContentAppService.GetAllByContentProviderId(id);
                if(getCommand!= null)
                {
                    return Ok(getCommand);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSpMappedToCpContentCountByContentProviderId([FromQuery]long id)
        {
            try
            {
                var getCommand = await _mapContentAppService.GetSpByContentProviderId(id);
                if (getCommand != null)
                {
                    return Ok(getCommand);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces(typeof(IEnumerable<MappedContentDto>))]
        public async Task<IActionResult> GetAllSpMappedToContentByContentProviderId([FromQuery]long serviceProviderId)
        {
            try
            {
                var getSps = await _mapContentAppService.GetAllSpMappedToAContent(serviceProviderId);
                if (getSps != null)
                {
                    return Ok(getSps);
                }
                return BadRequest(HttpStatusCode.BadRequest);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return NotFound(ErrorMessage.NOT_FOUND);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces(typeof(MappedContentDto))]
        public async Task<IActionResult> GetAllMappedContentByMapContentToServiceProviderId(long mapContentToServiceProviderId)
        {
            try
            {
                var mapContent = await _mapContentAppService.GetAllMappedContentByMapContentToServiceProviderId(mapContentToServiceProviderId);
                if (mapContent != null)
                {
                    return Ok(mapContent);
                }
                return NotFound(HttpStatusCode.NotFound);
            }

            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetServiceMappedToContentByContentId(long contentId)
        {
            try
            {
                var mapContent = await _mapContentAppService.GetServicesMappedToContentByContentId(contentId);
                
                    return Ok(mapContent);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex;
            }
        }

         /// <summary>
        /// Returns a list of distinct service providers mapped to any content
        /// for a particular content provider
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetDistinctSpMappedToCpContentByContentProviderId([FromQuery]long id)
        {
            try
            {
                var serviceProviders = await _mapContentAppService.GetDistinctSpforCpByContentProviderId(id);
                if (serviceProviders != null)
                {
                    return Ok(serviceProviders);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// This method returns a list of service providers mapped to a particular content
        /// </summary>
        /// <param name="cpId"></param>
        /// <param name="contentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAllSpMappedToCpContentByCPIdAndContentId([FromQuery]long cpId, long contentId)
        {
            try
            {
                var getServiceProviders = await _mapContentAppService.GetAllSPByCPIdAndContentId(cpId, contentId);
                if (getServiceProviders != null)
                {
                    return Ok(getServiceProviders);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw ex;
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Produces(typeof(bool))]
        public IActionResult IsContentRateMapped(long rateId)
        {
            try
            {
                var check =  _mapContentAppService.HasContentRateBeenMapped(rateId);
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
    }
}
