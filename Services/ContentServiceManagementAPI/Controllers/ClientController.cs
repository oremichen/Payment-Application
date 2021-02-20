using Anq.Dtos;
using Anq.ErrorMessages;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.ContentServiceLogger;
using ContentServiceManagementAPI.Models.DTO.Client;
using ContentServiceManagementAPI.Services.ClientAppService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Threading.Tasks;
using Anq.Enums;
using System.Collections.Generic;


namespace ContentServiceManagementAPI.Controllers
{   
    [ApiController]
    [Route("/api/[controller]")]
    public class ClientController : ControllerBase
    {
        #region Fields
        private readonly IClientAppService _clientAppService;
        private IHttpContextAccessor _httpContextAccessor;
        private ILogger _logger;
        //private readonly ILogger<ClientController>logger;
        private readonly ICServiceLogger cServiceLogger;
        #endregion

        #region Ctor
        public ClientController(IClientAppService clientAppService, ILogger<ClientController> logger,
            IHttpContextAccessor httpContextAccessor,
            ICServiceLogger CServiceLogger)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientAppService = clientAppService;
            _logger = logger;
            cServiceLogger = CServiceLogger;
            // this.logger = logs;
        }
        #endregion

        #region Methods


        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ClientDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Create(Client client)
        {
            try
            {
                var model = await _clientAppService.AddClient(client);
                if (model != null)
                {
                    var clientDto = _clientAppService.GetClientByAuthId(client.AuthenticationId);
                    return Ok(clientDto);
                }
            }
            catch (Exception e)
            {
                cServiceLogger.LogError(e.Message, e);
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("[action]")]
        [ProducesResponseType(typeof(ClientDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateAccountSetup(ClientDto clientDto)
        {
            try
            {
                var model = await _clientAppService.UpdateAccountSetup(clientDto);
                if (model != null)
                {
                    return Ok(clientDto);
                }
            }
            catch (Exception e)
            {
                cServiceLogger.LogError(e.Message, e);
                return BadRequest();
            }
            return BadRequest();
        }


        /// <summary>
        ///  Get a piginated list all the clients
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(Meta<ClientDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllClients(long? ApplicationId = (long)ApplicationEnum.ANQ,int startIndex = 0, int count = int.MaxValue)
        {
            int page = 0;
            int perpage = count;
            int pages = 1;
            int total = 0;
            int offset = startIndex;

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

                var getClients = await _clientAppService.GetAllClients();
                //getClients = getClients.Where(a => a.ApplicationId.Split(';').ToList().Contains(ApplicationId.ToString()));


                if (!string.IsNullOrEmpty(search))
                {
                    getClients = getClients.Where(x => x.AccountEmail.ToLower().Contains(search.ToLower())
                        || x.ClientName.ToLower().Contains(search.ToLower())
                        || x.ContactPersonEmail.ToLower().Contains(search.ToLower())
                        || x.ContactPersonFirstName.ToLower().Contains(search.ToLower())
                        || x.ContactPersonPhoneNumber.Contains(search)
                        || x.ContactPersonLastName.ToLower().Contains(search.ToLower())).ToList();
                }

                total = getClients.Count();

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

                    getClients = getClients.Skip(offset).Take(perpage);
                }


                //TODO: Clean up later
                var filtered = getClients;

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
                    filtered = filtered.OrderBy(x => x.ClientName);
                }

                var meta = new Meta<ClientDto>(dataList: filtered,
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
                cServiceLogger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ClientDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetClientById([FromQuery]long clientId)
        {
            try
            {
                var client = await _clientAppService.GetClientById(clientId);
                if (client == null)
                {
                    return NotFound(ErrorMessage.CUSTOM_Not_Found("Client"));
                }

                return Ok(client);
            }
            catch (Exception e)
            {
                //_logger.LogInformation(ex,ex.Message);

                cServiceLogger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }


        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IEnumerable<ClientDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetClientsByClientId([FromQuery]long clientId)
        {
            try
            {
                var client = await _clientAppService.GetClientById(clientId);
                if (client == null)
                {
                    return NotFound(ErrorMessage.CUSTOM_Not_Found("Client"));
                }

                return Ok(client);
            }
            catch (Exception e)
            {
                //_logger.LogInformation(ex,ex.Message);

                cServiceLogger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ClientDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetClientByAuthId([FromQuery]long authId)
        {
            try
            {
                var client = await _clientAppService.GetClientByAuthId(authId);
                if (client == null)
                {
                    return NotFound(ErrorMessage.CUSTOM_Not_Found("Client"));
                }

                return Ok(client);
            }
            catch (Exception e)
            {
                //_logger.LogInformation(ex,ex.Message);

                cServiceLogger.LogError(e.Message, e);
                return BadRequest(ErrorMessage.INTERNAL_SERVER_ERROR);
            }
        }

        #endregion
    }
}
