using Anq.Dtos;
using Anq.Enums;
using Anq.ErrorMessages;
using ANQ.Messages.NonScheduledTasks.ContentService.Models;
using ContentManagementServiceAPI.Models.Dto.ContentDto;
using ContentServiceManagementAPI.ContentServiceLogger;
using ContentServiceManagementAPI.Events.Publish.ServiceEvents;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO;
using ContentServiceManagementAPI.Models.DTO.Service;
using ContentServiceManagementAPI.Models.DTO.ServiceProvider;
using ContentServiceManagementAPI.Services.ServiceAppService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        #region Fields
        private readonly IServiceAppService _serviceAppService;
        private readonly IContentServiceEvent _contentServiceEvent;
        private readonly ILogger<ServiceController> _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private String _userName;
        private readonly ICServiceLogger _cServiceLogger;
        #endregion

        #region Ctor
        public ServiceController(
            IServiceAppService serviceAppService,
            ILogger<ServiceController> loggerFactory,
            IHttpContextAccessor httpContextAccessor,
            IContentServiceEvent contentServiceEvent,
            ICServiceLogger cServiceLogger)
        {
            _serviceAppService = serviceAppService;
            _logger = loggerFactory;
            _httpContextAccessor = httpContextAccessor;
            _userName = "Someone";
            _contentServiceEvent = contentServiceEvent;
            _cServiceLogger = cServiceLogger;
        }
        #endregion

        #region Action Method
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateServiceDto model)
        {
            long serviceId = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    bool checkName = await _serviceAppService.CheckIfNameExistForContentProvider(model.ServiceProviderId, model.ServiceName);
                    if (checkName == false)
                    {
                        var serviceExists = await _serviceAppService.CheckIfNameExist(model.ServiceName);

                        if (serviceExists)
                        {
                            return BadRequest("A service with name '" + model.ServiceName + "' already exists!");
                        }

                        if (model.ApplicationId < 1)
                        {
                            model.ApplicationId = (long)ApplicationEnum.ANQ;
                        }

                        var data = new Service()
                        {
                            ServiceName = model.ServiceName,
                            ServiceProviderId = model.ServiceProviderId,
                            ServiceProviderName = model.ServiceProviderName,
                            Active = model.Active,
                            CreatedOn = model.CreatedOn,
                            Note = model.Note,
                            Category = model.Category,
                            CategoryCode = model.CategoryCode,
                            Channel = model.Channel,
                            Industry = model.Industry,
                            Keyword = model.Keyword,
                            ShortCode = model.ShortCode,
                            Price = model.Price,
                            SubscriptionType = model.SubscriptionType,
                            Periodicity = model.Periodicity,
                            Frequency = model.Frequency,
                            ServiceStatus = model.ServiceStatus,
                            ActiveDate = model.ActiveDate,
                            ExpiryDate = model.ExpiryDate,
                            ApplicationId = model.ApplicationId,
                            OperatorIds = model.OperatorIds,
                            BillBasisId = model.BillBasisId,
                            BillPerTransactionCount = model.BillPerTransactionCount,
                            MobileOriginatingCount = model.MobileOriginatingCount,
                            MobileTerminatingCount = model.MobileTerminatingCount,
                            IsVasSystemService = model.IsVasSystemService == null ? false : model.IsVasSystemService,
                            VasSystemServiceCode = model.VasSystemServiceCode
                        };

                        if (data.CreatedOn != null)
                            data.CreatedOn = data.CreatedOn.ToLocalTime();

                        if (data.ActiveDate != null)
                            data.ActiveDate = data.ActiveDate.Value.ToLocalTime();

                        if (data.ActiveDate == null)
                            data.ActiveDate = DateTime.Now.ToLocalTime();

                        if (data.ExpiryDate != null)
                            data.ExpiryDate = data.ExpiryDate.Value.ToLocalTime();

                        serviceId = await _serviceAppService.Add(data);

                        if (model.ApplicationId == (int)ApplicationEnum.DND)
                        {
                            //Create Service Code
                            data.ServiceCode = model.ServiceProviderId + "-" + model.Industry + "-" + serviceId;
                            await _serviceAppService.AddServiceCode(serviceId, data.ServiceCode);

                            // post request to Dnd 
                            var serviceToPostToDnd = new ServiceDto
                            {
                                ServiceId = serviceId
                            };
                        }

         //-------------------------this is cannot be used, please ignore. Thank you-----------------------------//

                        //var service = new InsertServiceDto
                        //{
                        //    ServiceId = serviceId,
                        //    Balance = 0,
                        //    Owner = data.ServiceProviderName,
                        //    OwnerId = data.ServiceProviderId,
                        //    ServiceName = data.ServiceName,
                        //    Status = Anq.Enums.EntityStatus.InsufficientBalance,
                        //    ServiceProviderId = model.ServiceProviderId,
                        //    CreatedOn = model.CreatedOn,
                        //    Category = model.Category,
                        //    Channel = model.Channel,
                        //    Industry = model.Industry,
                        //    Keyword = model.Keyword,
                        //    ShortCode = model.ShortCode,
                        //    Price = model.Price,
                        //    SubscriptionType = model.SubscriptionType,
                        //    Periodicity = model.Periodicity,
                        //    Frequency = model.Frequency,
                        //    ServiceStatus = model.ServiceStatus,
                        //    ActiveDate = model.ActiveDate,
                        //    ExpiryDate = model.ExpiryDate,
                        //    ApplicationId = model.ApplicationId,
                        //    OperatorIds = model.OperatorIds,
                        //    BillBasisId = model.BillBasisId,
                        //    BillPerTransactionCount = model.BillPerTransactionCount,
                        //    MobileOriginatingCount = model.MobileOriginatingCount,
                        //    MobileTerminatingCount = model.MobileTerminatingCount
                        //};

                        // publish Service details
                        //await _contentServiceEvent.CreateServiceRequest(service);
         //---------------------------------------------------------------------------------------------------//

                        if (data.ApplicationId == (int)ApplicationEnum.ANQ)
                        {
                            data.BillBasisId = 0;
                            data.BillPerTransactionCount = 0;
                            data.Category = 0;
                            data.Channel = "";
                            data.Frequency = 0;
                            data.Industry = 0;
                            data.Keyword = "";
                            data.MobileOriginatingCount = 0;
                            data.MobileTerminatingCount = 0;
                            data.OperatorIds = "";
                            data.Periodicity = 0;
                            data.Price = 0;
                            data.ServiceCode = "";
                            data.ServiceStatus = 0;
                            data.ShortCode = "";
                            data.SubscriptionType = 0;
                            data.ActiveDate = DateTime.Now;
                            data.ExpiryDate = DateTime.Now;
                            data.IsVasSystemService = false;
                            data.VasSystemServiceCode = "";
                        }

                        bool add = await _serviceAppService.AddServiceOnContentProcessing(data);

                        if(add == true)
                        {
                           _logger.LogInformation($"Service was successfully created. PerformedBy{_userName}");
                            return Ok(data.ServiceId);
                        }
                        //return Ok(data.ServiceId);
                    }

                    //if (serviceId > 0)
                    //{
                    //    await _serviceAppService.Delete(new ServiceDto { ServiceId = serviceId, ServiceName = model.ServiceName });
                    //}
                    //return BadRequest("An error occurred during service creation");
                }


                //if publish to rabbit mq(subscribed services) did not go through,
                if (serviceId > 0)
                {
                    await _serviceAppService.Delete(new ServiceDto { ServiceId = serviceId, ServiceName = model.ServiceName });
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Service Controller=>Service Creation=> " + e.Message);
                //_logger.LogError(exception, $"Exception cuaght while trying to Map content create service ActionBy : {_userName}");

                _logger.LogError(e.Message, e);
                //return BadRequest(ex.Message);

                if (serviceId > 0)
                {
                    await _serviceAppService.Delete(new ServiceDto { ServiceId = serviceId, ServiceName = model.ServiceName });
                }
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update([FromBody] ServiceDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceExit = await _serviceAppService.GetById(model.ServiceId);
                    if (serviceExit != null)
                    {
                        var checkName = await _serviceAppService.CheckIfNameExistForContentProvider(model.ServiceProviderId, model.ServiceName);
                        if (checkName == false)
                        {
                            await _serviceAppService.Update(model.ServiceId);

                            _logger.LogInformation($"Service with id : {model.ServiceId} was succesffully updated. PerformedBy {_userName}");

                            var service = new InsertServiceDto
                            {
                                ServiceId = model.ServiceId,
                                Balance = 0,
                                Owner = model.ServiceProviderName,
                                OwnerId = model.ServiceProviderId,
                                ServiceName = model.ServiceName,
                                Status = Anq.Enums.EntityStatus.InsufficientBalance
                            };

                            // publish Service details
                            await _contentServiceEvent.CreateServiceRequest(service);

                            return Ok("Service Successfully updated");
                        }
                        return BadRequest(ErrorMessage.CUSTOM_Already_Exist(model.ServiceName));
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Exception caught Service Update failed. ServiceId : {model.ServiceId}. ActionBy : {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpDelete]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Remove(ServiceDto model)
        {
            try
            {
                var serviceToDelete = await _serviceAppService.GetById(model.ServiceId);
                if (serviceToDelete != null)
                {
                    await _serviceAppService.Delete(model);
                    _logger.LogInformation($"Service was deleted successfully. PerfromedBy {_userName}");
                    return Ok();
                }
                return NotFound(ErrorMessage.CUSTOM_Not_Found(model.ServiceName));
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
        [ProducesResponseType(typeof(ServiceDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                var service = await _serviceAppService.GetById(id);
                if (service != null)
                {
                    var serviceDto = new ServiceDto()
                    {
                        ServiceId = service.ServiceId,
                        ServiceName = service.ServiceName,
                        ServiceProviderId = service.ServiceProviderId,
                        ServiceProviderName = service.ServiceProviderName,
                        Note = service.Note,
                        CreatedOn = service.CreatedOn,
                        FormattedCreatedOnDate = service.CreatedOn.ToShortDateString(),
                        ServiceStatus = service.ServiceStatus,
                        SubscriptionType = service.SubscriptionType,
                        ServiceCode = service.ServiceCode,
                        Price = service.Price,
                        Frequency = service.Frequency,
                        ShortCode = service.ShortCode,
                        Keyword = service.Keyword,
                        ActiveDate = service.ActiveDate,
                        ExpiryDate = service.ExpiryDate,
                        Periodicity = service.Periodicity,
                        Industry = service.Industry,
                        Category = service.Category,
                        Channel = service.Channel
                    };

                    _logger.LogInformation($"Success! Service details retuned. ActionBy{_userName}");
                    return Ok(serviceDto);
                }
                return NotFound(ErrorMessage.NOT_FOUND);
            }
            catch (Exception e)
            {
                // _logger.LogError(ex, $"Exception caught! Service detail not returned. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces(typeof(IEnumerable<ServiceDtos>))]
        public async Task<IActionResult> GetServiceByServiceId(long id)
        {
            try
            {
                var service = await _serviceAppService.GetByServiceId(id);
                if (service != null)
                {
                    return Ok(service);
                }
                return BadRequest("Please contact administrator");

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw new SystemException($"{e}");
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(Meta<ServiceDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll([FromQuery] long? ApplicationId = (long)ApplicationEnum.ANQ)
        {
            int page = 0;
            int perpage = 25;
            int pages = 1;
            int total = 0;
            int offset = 0;

            try
            {
                var dataTableRequest = DataTableRequest.GetQueryStringForDataTable(_httpContextAccessor.HttpContext);

                var pageRequest = dataTableRequest.PageRequest ?? null;
                var perpageRequest = dataTableRequest.PerpageRequest ?? null;
                var search = dataTableRequest.Search ?? null;
                var sort = dataTableRequest.Sort ?? null;
                var field = dataTableRequest.Field ?? null;

                if (!string.IsNullOrEmpty(field))
                {
                    field = char.ToUpper(field.First()) + field.Substring(1);

                    if (field.Length < 4)
                    {
                        field = field.ToUpper();
                    }
                }

                if (!string.IsNullOrEmpty(pageRequest))
                {
                    page = Convert.ToInt32(pageRequest);
                }

                if (!string.IsNullOrEmpty(perpageRequest))
                {
                    perpage = Convert.ToInt32(perpageRequest);
                }

                var services = await _serviceAppService.GetAll((long)ApplicationId);

                if (!string.IsNullOrEmpty(search))
                {
                    services = services.Where(x => x.ServiceName.ToLower().Contains(search.ToLower())
                        || x.ServiceProviderName.ToLower().Contains(search.ToLower())
                        || x.Note.ToLower().Contains(search.ToLower())).ToList();

                }
                total = services.Count();

                if (perpage > 0)
                {
                    if (perpage < total)
                        pages = Convert.ToInt32(Math.Floor(Math.Ceiling(Convert.ToDouble(total / perpage))));

                    if (page <= 0)
                    {
                        page = Math.Max(page, 1);
                    }

                    else if (page > pages)
                    {
                        page = Math.Min(page, pages);
                    }

                    offset = (page - 1) * perpage;

                    if (offset < 0)
                        offset = 0;

                    services = services.Skip(offset).Take(perpage);
                }

                var filtered = services;

                if (!string.IsNullOrEmpty(sort))
                {
                    if (sort == "asc")
                    {
                        filtered = filtered.AsQueryable().OrderBy(field);
                    }
                    else if (sort == "desc")
                    {
                        filtered = filtered.AsQueryable().OrderBy($"{field} descending");
                    }
                }
                else
                {
                    filtered = filtered.AsQueryable().OrderBy(x => x.ServiceName);
                }

                var meta = new Meta<ServiceDto>(dataList: filtered,
                    dataPage: page,
                    dataPerpage: perpage,
                    dataPages: pages,
                    dataTotal: total,
                    dataSort: sort,
                    dataField: field);

                //return Ok(result);
                return Ok(meta);
            }
            catch (Exception e)
            {
                //_logger.LogError(ex, $"Exception caught! Service list not returned. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<ServiceDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllServiceNotMappedToClient(int id, long? ApplicationId = (long)ApplicationEnum.ANQ)
        {
            try
            {
                var data = await _serviceAppService.GetServicesNotMappedToClientByClientId(id, (long)ApplicationId);

                _logger.LogInformation($"Success! All Services list retuned. ActionBy{_userName}");
                return Ok(data);
            }
            catch (Exception e)
            {
                // _logger.LogError(ex, $"Exception caught! Service list not returned. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

       
        /// <summary>
        /// Get all Services by Service Provider
        /// </summary>
        /// <param name="serviceProviderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(Meta<ServiceDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllbyServiceProvideId(long serviceProviderId)
        {
            //TODO: First check if the ServiceProvider Id exists
            int page = 0;
            int perpage = 25;
            int pages = 1;
            int total = 0;
            int offset = 0;

            try
            {
                var dataTableRequest = DataTableRequest.GetQueryStringForDataTable(_httpContextAccessor.HttpContext);

                var pageRequest = dataTableRequest.PageRequest ?? null;
                var perpageRequest = dataTableRequest.PerpageRequest ?? null;
                var search = dataTableRequest.Search ?? null;
                var sort = dataTableRequest.Sort ?? null;
                var field = dataTableRequest.Field ?? null;

                if (!string.IsNullOrEmpty(field))
                {
                    field = char.ToUpper(field.First()) + field.Substring(1);

                    if (field.Length < 4)
                    {
                        field = field.ToUpper();
                    }
                }

                if (!string.IsNullOrEmpty(pageRequest))
                {
                    page = Convert.ToInt32(pageRequest);
                }

                if (!string.IsNullOrEmpty(perpageRequest))
                {
                    perpage = Convert.ToInt32(perpageRequest);
                }

                var services = await _serviceAppService.GetByServiceProviderId(serviceProviderId);

                if (!string.IsNullOrEmpty(search))
                {
                    services = services.Where(x => x.ServiceName.ToLower().Contains(search.ToLower())
                        || x.ServiceProviderName.ToLower().Contains(search.ToLower())
                        || x.Note.ToLower().Contains(search.ToLower())).ToList();

                }
                total = services.Count();

                if (perpage > 0)
                {
                    if (perpage < total)
                        pages = Convert.ToInt32(Math.Floor(Math.Ceiling(Convert.ToDouble(total / perpage))));

                    if (page <= 0)
                    {
                        page = Math.Max(page, 1);
                    }

                    else if (page > pages)
                    {
                        page = Math.Min(page, pages);
                    }

                    offset = (page - 1) * perpage;

                    if (offset < 0)
                        offset = 0;

                    services = services.Skip(offset).Take(perpage);
                    foreach (ServiceDto serv in services)
                    {
                        serv.Application = Enum.GetName(typeof(ApplicationEnum), serv.ApplicationId);
                    }
                }

                var filtered = services;
                if (!string.IsNullOrEmpty(sort))
                {
                    if (sort == "asc")
                    {
                        filtered = filtered.AsQueryable().OrderBy(field);
                    }
                    else if (sort == "desc")
                    {
                        filtered = filtered.AsQueryable().OrderBy($"{field} descending");
                    }
                }
                else
                {
                    filtered = filtered.AsQueryable().OrderBy(x => x.ServiceName);
                }

                var meta = new Meta<ServiceDto>(dataList: filtered,
                    dataPage: page,
                    dataPerpage: perpage,
                    dataPages: pages,
                    dataTotal: total,
                    dataSort: sort,
                    dataField: field);

                //return Ok(result);
                return Ok(meta);
            }
            catch (Exception e)
            {
                // _logger.LogError(ex, $"Exception caught! Service list not returned. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(serviceCountDto), (int)HttpStatusCode.OK)] [HttpGet]
        public async Task<IActionResult> GetServicesByServiceProviderIdPaginatedAndCount(long serviceProviderId, int skip = 0, int take = int.MaxValue, string search=null)
        {
            try
            {
                var result = await _serviceAppService.GetServicesByServiceProviderIdPaginatedAndCount(serviceProviderId, skip, take,search);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ServiceProviderServicesCount), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllServicesByServiceProvideId(long serviceProviderId, long? skip = null, long? take = null, string search = null)
        {
            try
            {
                var services = await _serviceAppService.GetByServiceProviderId(serviceProviderId,search);
                if (services != null)
                {
                    var serviceProviderServices = new ServiceProviderServicesCount()
                    {
                        Count = services.Count(),
                        //SpServices = services.Skip((int)skip).Take((int)take)
                        SpServices = services
                    };
                    return Ok(serviceProviderServices);
                }
                return BadRequest(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ex);
            }                   
        }

       

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(Service), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetbyShortCodekeyword(string shortcode, string keyword)
        {
            try
            {
                var Getshortkey = await _serviceAppService.GetbyShortCodekeyword(shortcode, keyword);
                if (Getshortkey == null)
                {
                    return NotFound("Not found");
                }
                else
                    return Ok(Getshortkey);
            }

            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<Service>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetByShortCodeAndServiceProviderId(string shortCode, long serviceProviderId)
        {
            try
            {

                var GetService = await _serviceAppService.GetByShortCodeAndServiceProviderId(shortCode, serviceProviderId);
                if (GetService == null)
                    return NotFound("Not found");
                else
                    return Ok(GetService);
            }

            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<Service>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SearchServices(ServiceSearchDto serviceSearchDto)
        {
            try
            {

                var Getshortkey = await _serviceAppService.SearchService(serviceSearchDto);
                if (Getshortkey == null)
                {
                    return NotFound("Not found");
                }
                else
                {
                    return Ok(Getshortkey);
                }

            }

            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(Service), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddServiceCode(Service service)
        {
            try
            {

                var addStatus = await _serviceAppService.AddServiceCode(service.ServiceId, service.ServiceCode);
                if (addStatus == true)
                {
                    return Ok(service);
                }
                else
                {
                    return BadRequest();
                }
            }

            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ValidateServiceShortCodeOperators(string ShortCode, string OperatorIds)
        {
            try
            {

                var Getshortkey = await _serviceAppService.ValidateServiceShortCodeOperators(ShortCode, OperatorIds);
                if (Getshortkey == null)
                {
                    return BadRequest("Not found");
                }
                else
                {
                    return Ok(Getshortkey);
                }

            }

            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }


        }


        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType(typeof(ServiceStatusReponseDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateServiceStatus(ServiceStatusReponseDto ServiceStatusRequest)
        {
            try
            {

                //In app Service, do conditions
                var Update = await _serviceAppService.UpdateServiceStatus(ServiceStatusRequest);
                return Ok(Update);
            }

            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }


        }

        [HttpGet]
        [Route("GetByServiceId/{serviceId}")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(ContentDto))]
        public async Task<IActionResult> GetByServiceId(long serviceId)
        {
            try
            {
                if (serviceId > 0)
                {
                    var service = await _serviceAppService.GetServiceByServiceId(serviceId);
                    if (service != null)
                    {
                        _logger.LogInformation($"Successful! All content was retrieved PerformedBy {_userName}");
                        return Ok(service);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.DOES_NOT_EXIST);
            }
            catch (Exception e)
            {
                //_logger.LogError(exception, $"Content with its {id} was not retrived. {exception.Message} PerformedBy - {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces(typeof(serviceCountDto))]
        public async Task<IActionResult> GetServiceBySpId(long serviceProviderId, int skip, int take)
        {
            try
            {
                //var count = await _serviceTransactionLogRepo.GetServiceTransactionByDateRangeCount(serviceId, channel, clientId, dateFrom, dateTo);
                var services = await _serviceAppService.GetServicesByServiceProviderId(serviceProviderId, skip, take);

                var count = _serviceAppService.GetServicesByServiceProviderIdCount(serviceProviderId);
                if (services != null && services.Count() > 0)
                {
                    var model = new serviceCountDto
                    {
                        Count = count,
                        Services = services

                    };
                    var status = new List<bool>();
                    if (services.Count() > 0)
                    {
                        foreach (var item in services)
                        {

                            var contents = await _serviceAppService.GetServiceContents(item.ServiceId);
                            if (contents.Count() > 0 && contents != null)
                            {

                                foreach (var con in contents)
                                {
                                    var result = item.Active = await _serviceAppService.GetContentServiceProviderBalance(serviceProviderId, con.ContentId);
                                    status.Add(result);
                                }

                                if (status.Any() == false)
                                {
                                    item.Active = false;
                                }
                                else
                                {
                                    item.Active = true;
                                }

                                status = new List<bool>();
                            }
                        }
                    }




                    return Ok(model);
                }
                return Ok(null) ;
            }
            catch (Exception ex)
            {
                throw new SystemException($"{ex}");
            }
            #endregion   
     }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Produces(typeof(ServiceDto))]
        public async Task<IActionResult> GetServiceBySpIdDropDown(long serviceProviderId, int skip, int take)
        {
            try
            {
                //var count = await _serviceTransactionLogRepo.GetServiceTransactionByDateRangeCount(serviceId, channel, clientId, dateFrom, dateTo);
                var services = await _serviceAppService.GetServicesBySPId(serviceProviderId);

                if (services != null)
                {
                    return Ok(services);
                }
                return Ok(null);
            }
            catch (Exception ex)
            {
                throw new SystemException($"{ex}");
            }


        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Testing()
        {

            return Ok();
        }
    }
}
