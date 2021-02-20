using Anq.Dtos;
using Anq.ErrorMessages;
using ContentServiceManagementAPI.ContentServiceLogger;
using ContentServiceManagementAPI.Models;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO;
using ContentServiceManagementAPI.Models.DTO.ContentProvider;
using ContentServiceManagementAPI.Services;
using ContentServiceManagementAPI.Services.ContentProviderService;
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
    public class ContentProviderController : ControllerBase
    {
        #region Fields
        private readonly IContentProviderAppService _contentProviderAppService;
        private readonly IContentAppService _contentService;
        private readonly ILogger<ContentProviderController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private String _userName;
        private readonly ICServiceLogger _cServiceLogger;


        #endregion

        #region Ctor

        public ContentProviderController(IContentProviderAppService contentProviderAppService,
            ILogger<ContentProviderController> loggerFactory, IHttpContextAccessor httpContextAccessor, ICServiceLogger cServiceLogger, IContentAppService contentService)
        {
            _contentProviderAppService = contentProviderAppService;
            _logger = loggerFactory;
            _httpContextAccessor = httpContextAccessor;
            _userName = "Someone";
            _cServiceLogger = cServiceLogger;
            _contentService = contentService;
        }
        #endregion


        #region Method

        [HttpGet]
        [ProducesResponseType(typeof(Meta<ContentProviderDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
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

                var contentProviders = await _contentProviderAppService.GetContentProviders();

                if (!string.IsNullOrEmpty(search))
                {
                    contentProviders = contentProviders.Where(x => x.AccountEmail.ToLower().Contains(search.ToLower())
                        || x.ContentProviderName.ToLower().Contains(search.ToLower())
                        || x.ContactPersonEmail.ToLower().Contains(search.ToLower())
                        || x.ContactPersonFirstName.ToLower().Contains(search.ToLower())
                        || x.ContactPersonPhoneNumber.Contains(search)
                        || x.ContactPersonLastName.ToLower().Contains(search.ToLower())).ToList();

                }

                #region Paging
                total = contentProviders.Count();

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

                    contentProviders = contentProviders.Skip(offset).Take(perpage);
                }

                var filtered = contentProviders;

                #endregion

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
                    filtered = filtered.OrderBy(x => x.ContentProviderName);
                }

                var meta = new Meta<ContentProviderDto>(dataList: filtered,
                    dataPage: page,
                    dataPerpage: perpage,
                    dataPages: pages,
                    dataTotal: total,
                    dataSort: sort,
                    dataField: field);

                //return Ok(result);
                return Ok(meta);

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"An error occured while pulling data from ANQ.  PerfromedBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<CommandRecord>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCommandRecordByClientId(int clientId)
        {
            try
            {
                var commandRecord = await _contentProviderAppService.GetCommandRecordsByClientId(clientId);
                return Ok(commandRecord);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"An error occured while pulling data from ANQ.  PerfromedBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ContentProvider), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetContentproviderIdByAuthId(long authId)
        {
            try
            {
                var commandRecord = await _contentProviderAppService.GetContentProviderIdByAuthId(authId);
                return Ok(commandRecord);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"An error occured while pulling data from ANQ.  PerfromedBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ContentProviderDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var contentProvider = await _contentProviderAppService.GetContentProvider(id);
                if (contentProvider != null)
                {
                    return Ok(contentProvider);
                }
                return NotFound(ErrorMessage.NOT_FOUND);

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"An error occured .");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        [HttpGet]
        [Route("GetByContentProviderId/{CpId}")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(ContentProviderDto))]
        public async Task<IActionResult> GetByContentProviderId(long CpId)
        {
            try
            {
                var contentProvider = await _contentProviderAppService.GetContentProvider(CpId);
                if (contentProvider != null)
                {
                    return Ok(contentProvider);
                }
                return NotFound(ErrorMessage.NOT_FOUND);

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"An error occured .");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// 
        /// Adds to or updates what is available
        /// </summary>
        /// <returns></returns>
        [Obsolete("No longer needed to Update Content Provider")]
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<ContentProviderDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Update(ContentProviderDto model)
        {
            try
            {
                // await _contentProviderAppService.UpdateContentProvider();

                _logger.LogInformation($"Successe! content prividers Updated as compared with the Old Anq. PerformedBy {_userName}");
                return Ok("Update Successful");

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"An error occured while updating record");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetByContentProviderAndContentByAuthId/{authId}")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(ContentProviderAndContentDto))]
        public async Task<IActionResult> GetByContentProviderAndContentByAuthId(long authId)
        {
            try
            {
                var contentProvider = await _contentProviderAppService.GetContentProviderByAuthId(authId);
                if (contentProvider != null)
                {
                    var content = await _contentService.GetContentsByContentProvider(contentProvider.ContentProviderId);
                    ContentProviderAndContentDto contentProviderAndContent = new ContentProviderAndContentDto();
                    contentProviderAndContent.contents = content;
                    contentProviderAndContent.ContentProvider = contentProvider;
                    return Ok(contentProviderAndContent);
                }
                return NotFound(ErrorMessage.NOT_FOUND);

            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, $"An error occured .");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }
        #endregion
    }
} 
