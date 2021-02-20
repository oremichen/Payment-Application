using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;
using Anq.Dtos;
using Anq.Enums;
using Anq.ErrorMessages;
using ContentServiceManagementAPI.ContentServiceLogger;
using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DTO;
using ContentServiceManagementAPI.Models.ViewModels;
using ContentServiceManagementAPI.Services.ServiceProviderService;
using ContentServiceManagementAPI.Services.ServiceAppService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContentServiceManagementAPI.Models.DTO.DeliveryService;
using ContentServiceManagementAPI.Services.VasProviderAppService;

namespace ContentServiceManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceProviderController : ControllerBase
    {
        #region Fields
        private readonly IServiceProviderAppService _serviceProviderAppService;
        private readonly IServiceAppService _serviceAppService;
        private readonly ILogger<ServiceProviderController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private String _userName;
        private readonly ICServiceLogger _cServiceLogger;
        private readonly IVasProviderAppService _vasProviderAppService;
        #endregion

        #region Ctor

        public ServiceProviderController(
            IServiceProviderAppService serviceProviderAppService,
            ILogger<ServiceProviderController> loggerFactory, 
            IServiceAppService serviceAppService, 
            IHttpContextAccessor httpContextAccessor, 
            ICServiceLogger cServiceLogger,
            IVasProviderAppService vasProviderAppService)
        {
            _httpContextAccessor = httpContextAccessor;
            _serviceProviderAppService = serviceProviderAppService;
            _logger = loggerFactory;
            _serviceAppService = serviceAppService;
            _userName = "SomeOne";
            _cServiceLogger = cServiceLogger;
            _vasProviderAppService = vasProviderAppService;
        }
        #endregion


        #region Method

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ServiceProviderDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create(ServiceProvider serviceProvider)
        {
            try
            {
                var model = await _serviceProviderAppService.AddServiceProvider(serviceProvider);
                if (model != null)
                {
                    ///var sp = await _serviceProviderAppService.GetServiceProviderByAuthId(serviceProvider.AuthenticationId);
                    return Ok(model);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest();
            }
            return BadRequest();
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<ServiceProviderDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(long? ApplicationId = (long)ApplicationEnum.ANQ)
        {
            int page = 0;
            int perpage = 0;
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

                var serviceProviders = await _serviceProviderAppService.GetServiceProviders();
                serviceProviders = serviceProviders.Where(a => a.ApplicationId.Split(';').ToList().Contains(ApplicationId.ToString()));
                if (!string.IsNullOrEmpty(search))
                {
                    serviceProviders = serviceProviders.Where(x => x.AccountEmail.ToLower().Contains(search.ToLower())
                        || x.ServiceProviderName.ToLower().Contains(search.ToLower())
                        || x.ContactPersonEmail.ToLower().Contains(search.ToLower())
                        || x.ContactPersonFirstName.ToLower().Contains(search.ToLower())
                        || x.ContactPersonPhoneNumber.Contains(search)
                        || x.ContactPersonLastName.ToLower().Contains(search.ToLower())).ToList();

                }

                total = serviceProviders.Count();

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

                    serviceProviders = serviceProviders.Skip(offset).Take(perpage);
                }

                var filtered = serviceProviders;
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
                    filtered = filtered.AsQueryable().OrderBy(x => x.ServiceProviderName);
                }

                var meta = new Meta<ServiceProviderDto>(dataList: filtered,
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
                // _logger.LogError(ex, $"Exception caught. Content Providers could not be fetch. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ServiceProviderDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetbyId(int id)
        {
            try
            {
                var serviceprovider = await _serviceProviderAppService.GetServiceProvider(id);
                if (serviceprovider == null)
                {
                    return NotFound(ErrorMessage.NOT_FOUND);
                }

                _logger.LogInformation($"success! content provider retrieved  fetched. actionby - {_userName}");
                return Ok(serviceprovider);
            }
            catch (Exception e)
            {
                //_logger.LogError(ex, $"exception caught. content provider could not be fetch. actionby {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        [HttpGet]
        [Route("GetByServiceProviderId/{sPId}")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(ContentProviderDto))]
        public async Task<IActionResult> GetByServiceProviderId(long sPId)
        {
            try
            {
                var contentProvider = await _serviceProviderAppService.GetServiceProvider(sPId);
                if (contentProvider != null)
                {
                    return Ok(contentProvider);
                }
                return NotFound(ErrorMessage.NOT_FOUND);

            }
            catch (Exception e)
            {
                //_logger.LogError(ex, $"An error occured .");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ServiceProviderDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetbyAuthId(int id)
        {
            try
            {
                var serviceprovider = await _serviceProviderAppService.GetServiceProviderByAuthId(id);
                if (serviceprovider == null)
                {
                    return NotFound(ErrorMessage.NOT_FOUND);
                }

                _logger.LogInformation($"success! service provider retrieved  fetched. actionby - {_userName}");
                return Ok(serviceprovider);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"exception caught. service provider could not be fetch. actionby {_userName}");
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        [Obsolete]
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<ServiceProviderDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update()
        {
            try
            {
                // await _serviceProviderAppService.UpdateServiceProvider();
                _logger.LogInformation($"Success! Content Provider updated across board. ActionBy - {_userName}");

                return Ok();
            }
            catch (Exception e)
            {

                //_logger.LogError(ex, $"Exception caught. Content Provider could not be fetch. ActionBy {_userName}");

                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);

            }

        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateApprovedStatus(ServiceProviderDto serviceProviderDto)
        {
            try
            {
                await _serviceProviderAppService.UpdateApprovedStatus(serviceProviderDto);
                if (serviceProviderDto.Approved == "A")
                {
                    await _serviceAppService.SetServiceProviderServiceStatus(serviceProviderDto, (int)ServiceStatus.Inactive);
                }
                return Ok(serviceProviderDto);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CompleteVasProviderConfiguration(VasProviderConfigurationDto vasProviderConfigurationDto)
        {
            try
            {
                await _vasProviderAppService.CompleteVasProviderConfiguration(vasProviderConfigurationDto);

                return Ok(vasProviderConfigurationDto);
            }
            catch (Exception e )
            {
                _logger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }
        #endregion


    }
}