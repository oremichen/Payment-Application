using Anq.Dtos;
using Anq.ErrorMessages;
using ContentManagementServiceAPI.Models.Dto.ContentDto;
using ContentServiceManagementAPI.ContentServiceLogger;
using ContentServiceManagementAPI.Models.DTO.Content;
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
    public class ContentController : ControllerBase
    {
        #region Fields
        private readonly IContentAppService _contentService;
        private readonly IContentProviderAppService _contentProviderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ContentController> _logger;
        private String _userName;
        private readonly ICServiceLogger _cServiceLogger;
        #endregion

        #region Ctor
        public ContentController(IContentAppService contentService, IContentProviderAppService contentProviderService,
            ILogger<ContentController> loggerFactory, IHttpContextAccessor httpContextAccessor, ICServiceLogger cServiceLogger)
        {
            _contentService = contentService;
            _contentProviderService = contentProviderService;
            _logger = loggerFactory;
            _httpContextAccessor = httpContextAccessor;
            _userName = "Someone";
            _cServiceLogger = cServiceLogger;
        }

        #endregion

        /// <summary>
        /// Create a content
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create([FromBody] CreateContentDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    bool checkName = await _contentService.CheckIfNameExist(model.Name);
                    if (checkName == false)
                    {
                        var intResult = await _contentService.Add(model);
                        return Ok(intResult);
                    }
                    return BadRequest(ErrorMessage.CUSTOM_Already_Exist(model.Name));
                }
                return BadRequest(ErrorMessage.Null_MODEL);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Error, {exception.Message} Caught perfromedBy- {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        ///  update Content by model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [Produces(typeof(long))]
        public async Task<IActionResult> Update([FromBody] ContentDto model)
        {
            try
            {
                if (model.ContentId > 0)
                {
                    // check if selected name has been used
                    //var checkContentName = await _contentService.CheckIfNameExistForContentProvider(model.ContentProviderId, model.Name);
                    //if (checkContentName)
                    //    return BadRequest(ErrorMessage.CUSTOM_Does_Not_Exist(model.Name));

                    if (ModelState.IsValid)
                    {
                        var contentId = await _contentService.Update(model);

                        _logger.LogInformation($"Content - {model.Name} updated successfully By- {_userName}");
                        return Ok(contentId);
                    }
                    return BadRequest(ErrorMessage.Null_MODEL);
                }

                return BadRequest(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Content Update failed. {exception.Message} User: {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// Delete a content by its id that is ContentId
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Remove(ContentDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var content = await _contentService.GetById(model.ContentId);
                    await _contentService.Delete(content);

                    _logger.LogInformation($"Content was deleted successfully. PerfromedBy {_userName}");
                    return Ok("Content Deleted Succefully");
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Content was not deleted. {exception.Message} PerfromedBy - {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);

            }
        }

        /// <summary>
        ///  Gets all content for by a contentProviderId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(Meta<ContentDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllByContentProviderId(long id)
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

                if (id > 0)
                {
                    // check if contentProvider exits first
                    var contentProvider = await _contentProviderService.GetContentProvider(id);
                    if (contentProvider == null)
                        return NotFound(ErrorMessage.NOT_FOUND);

                    // check datatable values
                    if (!string.IsNullOrEmpty(pageRequest))
                    {
                        page = Convert.ToInt32(pageRequest);
                    }

                    if (!string.IsNullOrEmpty(perpageRequest))
                    {
                        perpage = Convert.ToInt32(perpageRequest);
                    }


                    var contents = await _contentService.GetContentsByContentProvider(id);

                    if (!string.IsNullOrEmpty(search))
                    {
                        contents = contents.Where(x => x.Name.ToLower().Contains(search.ToLower())
                            || x.Description.ToLower().Contains(search.ToLower()))
                            .ToList();
                    }

                    total = contents.Count();

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

                        contents = contents.Skip(offset).Take(perpage);
                    }


                    var filtered = contents;

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
                        filtered = filtered.AsQueryable().OrderBy(x => x.Name);
                    }

                    var meta = new Meta<ContentDto>(dataList: filtered,
                        dataPage: page,
                        dataPerpage: perpage,
                        dataPages: pages,
                        dataTotal: total,
                        dataSort: sort,
                        dataField: field);

                    //return Ok(result);
                    return Ok(meta);

                }
                return BadRequest(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Error. Contents not fetched for the contentProvier-{id}... PerformedBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        /// <summary>
        ///  Gets all content for by a contentProviderId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(Meta<ContentDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetContentsByContentProviderId(long id, int skip, int take)
        {
            try
            {
                ContentAndCountDto contentAndCount = new ContentAndCountDto();
                if (id > 0)
                {
                    // check if contentProvider exits first
                    var contentProvider = await _contentProviderService.GetContentProvider(id);
                    if (contentProvider == null)
                        return NotFound(ErrorMessage.NOT_FOUND);

                    contentAndCount.Contents = await _contentService.GetContentsByContentProvider(id);

                    contentAndCount.Count = contentAndCount.Contents.Count();



                    var meta = new ContentAndCountDto();

                    meta.Contents = contentAndCount.Contents.Skip(skip).Take(take);
                    meta.Count = contentAndCount.Contents.Count();
                    return Ok(meta);

                }
                return BadRequest(ErrorMessage.NOT_FOUND);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Error. Contents not fetched for the contentProvier-{id}... PerformedBy {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        /// <summary>
        /// This Gets all contents created.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(Meta<ContentDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
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


                var contents = await _contentService.GetAll();

                if (!string.IsNullOrEmpty(search))
                {
                    contents = contents.Where(x => x.Name.ToLower().Contains(search.ToLower())
                        || x.Description.ToLower().Contains(search.ToLower()))
                        .ToList();
                }

                total = contents.Count();

                if (perpage > 0)
                {
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

                    contents = contents.Skip(offset).Take(perpage);
                }

                var filtered = contents;

                if (!string.IsNullOrEmpty(sort))
                {
                    if (sort == "asc")
                    {
                        filtered = filtered.AsQueryable().OrderBy(field);
                    }
                    else if (sort == "desc")
                    {
                        filtered = filtered.AsQueryable().OrderBy(field);
                    }
                }
                else
                {
                    filtered = filtered.AsQueryable().OrderBy(x => x.Name);
                }

                var meta = new Meta<ContentDto>(dataList: filtered,
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
                // _logger.LogError(exception, $"Error!- Contents were not fetched. {exception.Message} Exception caught {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR); ;
            }
        }

        /// <summary>
        /// Get a Content by its identifier - ContentId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(ContentDto))]
        public async Task<IActionResult> GetByContentId(long id)
        {
            try
            {
                if (id > 0)
                {
                    var content = await _contentService.GetById(id);
                    if (content != null)
                    {
                        _logger.LogInformation($"Successful! All content was retrieved PerformedBy {_userName}");
                        return Ok(content);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.DOES_NOT_EXIST);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Content with its {id} was not retrived. {exception.Message} PerformedBy - {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        [HttpGet]
        [Route("GetContentByContentId/{contentId}")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(ContentDto))]
        public async Task<IActionResult> GetContentByContentId(long contentId)
        {
            try
            {
                if (contentId > 0)
                {
                    var content = await _contentService.GetById(contentId);
                    if (content != null)
                    {
                        _logger.LogInformation($"Successful! All content was retrieved PerformedBy {_userName}");
                        return Ok(content);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.DOES_NOT_EXIST);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Content with its {id} was not retrived. {exception.Message} PerformedBy - {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpGet]
        [Route("GetContentRateByContentProviderId/{ContentProviderId}")]
        [ProducesResponseType((int)HttpStatusCode.Found)]
        [Produces(typeof(ContentDto))]
        public async Task<IActionResult> GetContentByContentProviderId(long ContentProviderId)
        {
            try
            {
                if (ContentProviderId > 0)
                {
                    var content = await _contentService.GetContentsByContentProvider(ContentProviderId);
                    if (content != null)
                    {
                        _logger.LogInformation($"Successful! All content was retrieved PerformedBy {_userName}");
                        return Ok(content);
                    }
                    return NotFound(ErrorMessage.NOT_FOUND);
                }
                return BadRequest(ErrorMessage.DOES_NOT_EXIST);
            }
            catch (Exception ex)
            {
                //_logger.LogError(exception, $"Content with its {id} was not retrived. {exception.Message} PerformedBy - {_userName}");

                _cServiceLogger.LogError(ex.Message, ex);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


    }
}

