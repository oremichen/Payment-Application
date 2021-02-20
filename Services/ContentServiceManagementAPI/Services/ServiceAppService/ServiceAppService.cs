using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Threading.Tasks;
using Anq.Enums;
using ContentServiceManagementAPI.Data;
using ContentServiceManagementAPI.Helpers;
using ContentServiceManagementAPI.Models.DomainModels;
using ContentServiceManagementAPI.Models.DTO.Service;
using ContentServiceManagementAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using ContentServiceManagementAPI.Models.DTO.ServiceContentMapping;
using ContentServiceManagementAPI.Models.DTO.ServiceProvider;
using Microsoft.Extensions.Options;
using ContentServiceManagementAPI.DapperImplementation;

namespace ContentServiceManagementAPI.Services.ServiceAppService
{
    public class ServiceAppService : IServiceAppService
    {
        private ANQContentServiceManageDb _aNQContentServiceManageDb;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ServiceAppService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly IServiceRepository _serviceRepository;
        private WalletAppSettings _walletAppSettings;
        private AppSettings _appSettings;


        public ServiceAppService(
            ANQContentServiceManageDb aNQContentServiceManageDb,
            IHttpContextAccessor httpContextAccessor , 
            ILogger<ServiceAppService> logger, 
            IHttpClientFactory httpClientFactory,
            IOptions<WalletAppSettings> walletAppService,
            IOptions<AppSettings> contentProcessingOptions,
            IConfiguration config, IServiceRepository serviceRepository)
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _walletAppSettings = walletAppService.Value;
            _appSettings = contentProcessingOptions.Value;
            _serviceRepository = serviceRepository;
        }
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("ANQContentServiceManageDbConnection"));
            }
        }


        public async Task<IEnumerable<ServiceDtos>> GetByServiceId(long serviceId)
        {
            try
            {
                using (IDbConnection conn = Connection)
                {
                    string sql = $"[dbo].[spGetAllServiceByServiceId] @serviceId";
                    conn.Open();
                    var result = conn.Query<ServiceDtos>(sql, new
                    {
                        serviceId
                    });
                    return await Task.FromResult(result);
                }
               
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw new SystemException($"{e}");
            }
        }

        public async Task<ServiceDtos> GetServiceByServiceId(long serviceId)
        {
            try
            {
                using (IDbConnection conn = Connection)
                {
                    string sql = $"[dbo].[spGetAllServiceByServiceId] @serviceId";
                    conn.Open();
                    var result = conn.Query<ServiceDtos>(sql, new
                    {
                        serviceId
                    }).FirstOrDefault();
                    return await Task.FromResult(result);
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw new SystemException($"{e}");
            }
        }



        public async Task<long> Add(Service model)
        {
            try
            {               
                if(model.ApplicationId == (long)ApplicationEnum.DND)
                {
                    
                    if (model.ServiceStatus != (int)ServiceStatus.Suspended)
                    {
                        //if todays date, active, else inactive
                        if (DateHelpers.IsSameDay((DateTime)model.ActiveDate, DateTime.Now))
                        {
                            model.ServiceStatus = (int)ServiceStatus.Active;
                        }
                        else
                        {
                            model.ServiceStatus = (int)ServiceStatus.Inactive;
                        }
                    }
                }

                await _aNQContentServiceManageDb.Service.AddAsync(model);
                _logger.LogInformation($"Appservice Created a new service with id {model.ServiceId}");
                await this.SaveAsync();
                return model.ServiceId;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Service AppService=>Service Creation=> " + e.Message);
                _logger.LogError(e.Message,e);
                return 0;    
            }
        }

        public async Task Update(long id)
        {
            try
            {
                var update = await _aNQContentServiceManageDb.Service.FindAsync(id);
                _aNQContentServiceManageDb.Entry(update).State = EntityState.Modified;

                await this.SaveAsync();
                _logger.LogInformation($"Update of the enitity with id {id} was successfull");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }

        public async Task Delete(ServiceDto entity)
        {
            try
            {
                var serviceToDelete = await _aNQContentServiceManageDb.Service.FindAsync(entity.ServiceId);
                if(serviceToDelete != null)
                {
                    _aNQContentServiceManageDb.Service.Remove(serviceToDelete);
                    await this.SaveAsync();
                    _logger.LogInformation($"AppService Deleted a service with name {entity.ServiceName}");
                }      
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }

        public async Task<IEnumerable<ServiceDto>> GetAll(long ApplicationId)
        {
            try
            {
                var services = await _aNQContentServiceManageDb.Service.Where(a=>a.ApplicationId == ApplicationId).OrderBy(x => x.ServiceId).ToListAsync();
                var servicesDto = new List<ServiceDto>();
                if (services != null)
                {
                    servicesDto.AddRange(services.OrderBy(x => x.CreatedOn).Select(service => new ServiceDto()
                    {
                        ServiceId = service.ServiceId,
                        ServiceName = service.ServiceName,
                        ServiceProviderId = service.ServiceProviderId,
                        Active = service.Active,
                        CreatedOn = service.CreatedOn,
                        FormattedCreatedOnDate = service.CreatedOn.ToShortDateString(),
                        Note = service.Note,
                        ServiceProviderName = service.ServiceProviderName,
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
                        Channel = service.Channel,
                        BillBasisId = service.BillBasisId,
                        BillPerTransactionCount = service.BillPerTransactionCount,
                        MobileOriginatingCount = service.MobileOriginatingCount,
                        MobileTerminatingCount = service.MobileTerminatingCount,
                        OperatorIds = service.OperatorIds,
                        ApplicationId = service.ApplicationId,
                        IsVasSystemService = service.IsVasSystemService,
                        VasSystemServiceCode = service.VasSystemServiceCode,
                        CategoryCode = service.CategoryCode
                    }));                  
                }
                return servicesDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw e;
            }
        }

       
        

        public async Task<Service> GetbyShortCodekeyword(string shortcode, string keyword)
        {
            try
            {

                if (keyword == null)
                {
                    keyword = "";
                }
                //return await _aNQContentServiceManageDb.Service.Where(s => s.ShortCode == shortcode || s.Keyword == keyword);
                return await _aNQContentServiceManageDb.Service.FirstOrDefaultAsync(s => s.ShortCode == shortcode && s.Keyword==keyword);

            }
            catch(Exception e)
            {
                _logger.LogError(e.Message,e);
                return null;
            }
           
        }

        public async Task<IEnumerable<ServiceDto>> GetServicesNotMappedToClientByClientId(int id, long ApplicationId)
        {
            try
            {
                var getAllServices = await this.GetAll(ApplicationId);

                var mappedServicestoClients = await _aNQContentServiceManageDb.MapServiceToClient.OrderBy(x => x.ClientId).ToListAsync();
                
                var data = getAllServices.Where(c => !mappedServicestoClients.Any(x => c.ServiceId == x.ServiceId)).OrderByDescending(order => order.ServiceId);

                if (data != null)
                {
                    var notMappedServices = new List<ServiceDto>();
                    notMappedServices.AddRange(data.OrderBy(x => x.ServiceId).Select(x => new ServiceDto()
                    {
                        ServiceId = x.ServiceId,
                        ServiceName= x.ServiceName,
                        ServiceProviderId = x.ServiceProviderId,
                        CreatedOn = x.CreatedOn,
                        ServiceProviderName = x.ServiceProviderName,
                        FormattedCreatedOnDate = x.CreatedOn.ToShortDateString(),
                        Note = x.Note,
                        ServiceStatus = x.ServiceStatus,
                        SubscriptionType = x.SubscriptionType,
                        ServiceCode = x.ServiceCode,
                        Price = x.Price,
                        Frequency = x.Frequency,
                        ShortCode = x.ShortCode,
                        Keyword = x.Keyword,
                        ActiveDate = x.ActiveDate,
                        ExpiryDate = x.ExpiryDate,
                        Periodicity = x.Periodicity,
                        Industry = x.Industry,
                        Category = x.Category,
                        Channel = x.Channel,
                        BillBasisId = x.BillBasisId,
                        BillPerTransactionCount = x.BillPerTransactionCount,
                        MobileOriginatingCount = x.MobileOriginatingCount,
                        MobileTerminatingCount = x.MobileTerminatingCount,
                        OperatorIds = x.OperatorIds,
                        ApplicationId = x.ApplicationId,
                        IsVasSystemService = x.IsVasSystemService,
                        VasSystemServiceCode = x.VasSystemServiceCode,
                        CategoryCode = x.CategoryCode
                    }));
                    return notMappedServices;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        //public async Task<PaginatedItemsViewModel<ServiceDto>> GetAll(int pagesize, int pagenumber, string searchstring)
        //{
        //    try
        //    {
        //        var services = new List<Service>();
        //        if (string.IsNullOrEmpty(searchstring) == false)
        //        {
        //            searchstring = searchstring.ToLower().Trim();
        //            services = await _aNQContentServiceManageDb.Service.Where(s => s.ServiceName.ToLower().Contains(searchstring) == true ||
        //                                                s.Note.ToLower().Contains(searchstring) == true ||
        //                                                s.CreatedOn.ToString().ToLower().Contains(searchstring) == true).ToListAsync();
        //        }
        //        else
        //        {
        //            services = await _aNQContentServiceManageDb.Service.OrderByDescending(or => or.CreatedOn).ToListAsync();
        //        }
        //        var paggedData = new PaginatedItemsViewModel<ServiceDto>();
        //        if (services.Count() > 0)
        //        {
        //            var servicesDto = new List<ServiceDto>();
        //            servicesDto.AddRange(services.OrderBy(x => x.CreatedOn).Select(service => new ServiceDto()
        //            {
        //                ServiceId = service.ServiceId,
        //                ServiceName = service.ServiceName,
        //                ServiceProviderId = service.ServiceProviderId,
        //                Active = service.Active,
        //                CreatedOn = service.CreatedOn,
        //                Note = service.Note,
        //            }));

        //            paggedData = Pagination.PaggedResult(servicesDto, pagesize, pagenumber);
        //            return paggedData;
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Excepiton caught , please review your entry or contact your administrator");
        //        throw ex;
        //    }
        //}

        public async Task<Service> GetById(long id)
        {
            try
            {
                var service =  _aNQContentServiceManageDb.Service.Find(id);

                return service;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task<serviceCountDto> GetServicesByServiceProviderIdPaginatedAndCount(long serviceProviderId, int skip = 0, int take = int.MaxValue, string search = null)
        {

            var services = await _serviceRepository.GetServiceWithSkipTakAndPagination(spId: serviceProviderId, skip: skip, take: take, keyword: search);
            var servicesCount = await _serviceRepository.GetServiceWithCountAndPagination(spId: serviceProviderId, keyword: search);

            var serviceModelDto = new serviceCountDto
            {
                Count = (int)servicesCount,
                Services = services
            };
            return serviceModelDto;
        }

        public async Task<IEnumerable<ServiceDto>> GetByServiceProviderId(long serviceProviderId,string search = null)
        {
            try
            {
                var servicesDto = new List<ServiceDto>();
               var services = await _aNQContentServiceManageDb.Service.Where(s => s.ServiceProviderId == serviceProviderId).ToListAsync();
               
               
                    servicesDto.AddRange(services.OrderByDescending(x => x.CreatedOn).Select(service => new ServiceDto()
                    {
                        ServiceId = service.ServiceId,
                        ServiceName = service.ServiceName,
                        ServiceProviderId = service.ServiceProviderId,
                        Active = service.Active,
                        CreatedOn = service.CreatedOn,
                        FormattedCreatedOnDate = service.CreatedOn.ToShortDateString(),
                        Note = service.Note,
                        ServiceProviderName = service.ServiceProviderName,
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
                        Channel = service.Channel,
                        BillBasisId = service.BillBasisId,
                        BillPerTransactionCount = service.BillPerTransactionCount,
                        MobileOriginatingCount = service.MobileOriginatingCount,
                        MobileTerminatingCount = service.MobileTerminatingCount,
                        OperatorIds = service.OperatorIds,
                        ApplicationId = service.ApplicationId,
                        IsVasSystemService = service.IsVasSystemService,
                        VasSystemServiceCode = service.VasSystemServiceCode,
                        CategoryCode = service.CategoryCode
                    }));

                return servicesDto;

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
        }

        //public async Task<serviceCountDto> GetServicesByServiceProviderIdPaginatedAndCount(long serviceProviderId, int skip = 0, int take = int.MaxValue,string search= null)
        //{
            
        //    var services = await _serviceRepository.GetServiceWithSkipTakAndPagination(spId: serviceProviderId, skip: skip, take: take, keyword: search);
        //    var servicesCount = await _serviceRepository.GetServiceWithCountAndPagination(spId: serviceProviderId, keyword: search);
            
        //    var serviceModelDto = new serviceCountDto
        //    {
        //        Count = (int)servicesCount,
        //        Services = services
        //    };
        //    return serviceModelDto;
        //}

        public async Task<List<string>> ValidateServiceShortCodeOperators(string ShortCode, string OperatorIds)
        {
            try
            {
                List<string> ExistingOperatorsForThisShortCode = new List<string>();
                var Services = _aNQContentServiceManageDb.Service.Where(a => a.ShortCode == ShortCode);

                foreach(var Service in Services)
                {
                    if (Service != null)
                    {
                        List<string> OperatorIdList = OperatorIds.Split(';').ToList();
                        foreach (string OperatorId in OperatorIdList)
                        {
                            if (Service.OperatorIds != null)
                            {
                                if (Service.OperatorIds.Split(';').ToList().Contains(OperatorId))
                                {
                                    ExistingOperatorsForThisShortCode.Add(OperatorId);
                                }
                            }
                        }
                    }
                }
                

                return ExistingOperatorsForThisShortCode;
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task<bool> AddServiceCode(long ServiceId, string ServiceCode)
        {
            try
            {
                var Service = _aNQContentServiceManageDb.Service.FirstOrDefault(a => a.ServiceId == ServiceId);
                if(Service != null)
                {
                    Service.ServiceCode = ServiceCode;

                    _aNQContentServiceManageDb.Service.Update(Service);
                    int count = _aNQContentServiceManageDb.SaveChanges();
                    if (count > 0)
                    {
                        bool Update = await AddServiceCodeOnContentProcessing(new Service
                        {
                            ServiceId = ServiceId,
                            ServiceCode = ServiceCode
                        });

                        return Update;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Service AppService=>Service Code Creation=> " + e.Message);
                _logger.LogError(e.Message, e);
                return false;
            }
            return false;
        }

        public async Task<bool> SetServiceProviderServiceStatus (ServiceProviderDto _serviceProviderDto, int ServiceStatus)
        {
            try
            {
                //var SPStatus = await _aNQContentServiceManageDb.ServiceProvider.FirstOrDefaultAsync(a => a.ServiceProviderId == _serviceProviderDto.ServiceProviderId);
                var SPServices =  _aNQContentServiceManageDb.Service.Where(a => a.ServiceProviderId == _serviceProviderDto.ServiceProviderId);

                foreach(var service in SPServices)
                {
                    if (service != null)
                    {
                        var _Service = _aNQContentServiceManageDb.Service.FirstOrDefault(a => a.ServiceId == service.ServiceId);
                        _Service.ServiceStatus = ServiceStatus;
                        _aNQContentServiceManageDb.Service.Update(_Service);
                        int save = _aNQContentServiceManageDb.SaveChanges();

                        if (save > 0)
                        {
                            //add api call here
                            UpdateServiceStatusDto updateServiceStatusDto = new UpdateServiceStatusDto();
                            updateServiceStatusDto.ActiveDate = _Service.ActiveDate;
                            updateServiceStatusDto.ExpiryDate = _Service.ExpiryDate;
                            updateServiceStatusDto.ServiceId = _Service.ServiceId;
                            updateServiceStatusDto.ServiceStatus = (int)_Service.ServiceStatus;

                            bool Sent = await UpdateOnContentProcessing(updateServiceStatusDto);
                            if (Sent)
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                return false;
            }

            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return false;
            }

        }

        public async Task<bool> UpdateServiceStatus(ServiceDto _service)
        {
            try
            {
                
                var Service = await _aNQContentServiceManageDb.Service.FirstOrDefaultAsync(a => a.ServiceId == _service.ServiceId);
                if (Service != null)
                {
                    Service.ServiceStatus = _service.ServiceStatus;

                    _aNQContentServiceManageDb.Service.Update(Service);
                    int count = _aNQContentServiceManageDb.SaveChanges();

                    if (count > 0)
                    {
                        //add api call here
                        UpdateServiceStatusDto updateServiceStatusDto = new UpdateServiceStatusDto();
                        updateServiceStatusDto.ActiveDate = Service.ActiveDate;
                        updateServiceStatusDto.ExpiryDate = Service.ExpiryDate;
                        updateServiceStatusDto.ServiceId = Service.ServiceId;
                        updateServiceStatusDto.ServiceStatus = (int)Service.ServiceStatus;

                        bool Sent = await UpdateOnContentProcessing(updateServiceStatusDto);
                        if (Sent)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
                return false;
            }
            return false;
        }

        public async Task<ServiceStatusReponseDto> UpdateServiceStatus(ServiceStatusReponseDto ServiceStatusRequest)
        {
            try
            {
                //Get Service
                var Service = _aNQContentServiceManageDb.Service.FirstOrDefault(a => a.ServiceId == ServiceStatusRequest.ServiceId);

                if(Service != null)
                {
                    var NewStatus = (ServiceStatus)ServiceStatusRequest.ServiceStatus;
                    
                    if(NewStatus == ServiceStatus.Active)
                    {
                        //If the Person wants to change to Ative, 
                        Service.ServiceStatus = (int)NewStatus;
                        Service.ExpiryDate = ServiceStatusRequest.ExpiryDate;

                        var PreviousStatus = (ServiceStatus)ServiceStatusRequest.PreviousServiceStatus;
                        if(PreviousStatus == ServiceStatus.Inactive)
                        {
                            Service.ActiveDate = DateTime.Now;
                        }
                        

                        _aNQContentServiceManageDb.Service.Update(Service);
                        int count = _aNQContentServiceManageDb.SaveChanges();

                        if (count > 0)
                        {
                            UpdateServiceStatusDto updateServiceStatusDto = new UpdateServiceStatusDto();
                            updateServiceStatusDto.ActiveDate = Service.ActiveDate;
                            updateServiceStatusDto.ExpiryDate = Service.ExpiryDate;
                            updateServiceStatusDto.ServiceId = Service.ServiceId;
                            updateServiceStatusDto.ServiceStatus = (int)Service.ServiceStatus;

                            bool update = await UpdateOnContentProcessing(updateServiceStatusDto);

                            if (update)
                            {
                                return new ServiceStatusReponseDto()
                                {
                                    ServiceStatus = (int)NewStatus,
                                    Successful = true,
                                    StartDate = Service.ActiveDate,
                                    ExpiryDate = Service.ExpiryDate,
                                    Message = "Your service is now Active!",
                                    ServiceId = Service.ServiceId
                                };
                            }                            
                        }                                                                      
                    }

                    if (NewStatus == ServiceStatus.Inactive)
                    {
                        //If the Person wants to change to InActive, 
                        Service.ServiceStatus = (int)NewStatus;
                        Service.ExpiryDate = DateTime.Now;

                        _aNQContentServiceManageDb.Service.Update(Service);
                        int count = _aNQContentServiceManageDb.SaveChanges();

                        if (count > 0)
                        {
                            UpdateServiceStatusDto updateServiceStatusDto = new UpdateServiceStatusDto();
                            updateServiceStatusDto.ActiveDate = Service.ActiveDate;
                            updateServiceStatusDto.ExpiryDate = Service.ExpiryDate;
                            updateServiceStatusDto.ServiceId = Service.ServiceId;
                            updateServiceStatusDto.ServiceStatus = (int)Service.ServiceStatus;

                            bool update = await UpdateOnContentProcessing(updateServiceStatusDto);

                            if(update)
                            {
                                return new ServiceStatusReponseDto()
                                {
                                    ServiceStatus = (int)NewStatus,
                                    Successful = true,
                                    StartDate = Service.ActiveDate,
                                    ExpiryDate = Service.ExpiryDate,
                                    Message = "Your service is now Inactive!",
                                    ServiceId = Service.ServiceId
                                };
                            }                          
                        }
                    }

                    if (NewStatus == ServiceStatus.Partial)
                    {
                        //If the Person wants to change to InActive, 
                        Service.ServiceStatus = (int)NewStatus;

                        _aNQContentServiceManageDb.Service.Update(Service);
                        int count = _aNQContentServiceManageDb.SaveChanges();

                        if (count > 0)
                        {
                            UpdateServiceStatusDto updateServiceStatusDto = new UpdateServiceStatusDto
                            {
                                ActiveDate = Service.ActiveDate,
                                ExpiryDate = Service.ExpiryDate,
                                ServiceId = Service.ServiceId,
                                ServiceStatus = (int)Service.ServiceStatus
                            };

                            bool update = await UpdateOnContentProcessing(updateServiceStatusDto);

                            if (update)
                            {
                                return new ServiceStatusReponseDto()
                                {
                                    ServiceStatus = (int)NewStatus,
                                    Successful = true,
                                    StartDate = Service.ActiveDate,
                                    ExpiryDate = Service.ExpiryDate,
                                    Message = "Your service is now Partial!",
                                    ServiceId = Service.ServiceId
                                };
                            }
                        }
                    }

                    if (NewStatus == ServiceStatus.Suspended)
                    {
                        //If the Person wants to change to InActive, 
                        Service.ServiceStatus = (int)NewStatus;

                        _aNQContentServiceManageDb.Service.Update(Service);
                        int count = _aNQContentServiceManageDb.SaveChanges();

                        if (count > 0)
                        {
                            UpdateServiceStatusDto updateServiceStatusDto = new UpdateServiceStatusDto
                            {
                                ActiveDate = Service.ActiveDate,
                                ExpiryDate = Service.ExpiryDate,
                                ServiceId = Service.ServiceId,
                                ServiceStatus = (int)Service.ServiceStatus
                            };

                            bool update = await UpdateOnContentProcessing(updateServiceStatusDto);

                            if(update)
                            {
                                return new ServiceStatusReponseDto()
                                {
                                    ServiceStatus = (int)NewStatus,
                                    Successful = true,
                                    StartDate = Service.ActiveDate,
                                    ExpiryDate = Service.ExpiryDate,
                                    Message = "Your service is now Suspended!",
                                    ServiceId = Service.ServiceId
                                };
                            }                                                      
                        }
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message, e);
                return new ServiceStatusReponseDto()
                {
                    Successful = false
                };
            }
            return new ServiceStatusReponseDto()
            {
                Successful = false
            };
        }

        public async Task<bool> UpdateOnContentProcessing(UpdateServiceStatusDto updateServiceStatusDto)
        {
            try
            {
                var httpservice = _httpClientFactory.CreateClient("ContentProcessingService");
                var result = await httpservice.PutAsync("api/Service/UpdateServiceStatusByServiceId", new JsonContent(updateServiceStatusDto));
                if (result.IsSuccessStatusCode)
                {
                    var httpresponse = await result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<UpdateServiceStatusDto>(httpresponse);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return false;
            }
        }

        public async Task<bool> AddServiceOnContentProcessing(Service service)
        {
            try
            {
                Console.WriteLine("Entered api request");
                var httpservice = _httpClientFactory.CreateClient("ContentProcessingService");

                string requestUri = string.Format($"{_appSettings.BaseUrl}" + "/api/Service/CreateServiceFromAnq");

                //var requestUrl = httpservice.BaseAddress.AbsoluteUri + "api/Service/Create";

                var result = await httpservice.PostAsJsonAsync(requestUri, service);
                if (result.IsSuccessStatusCode)
                {
                    var httpresponse = await result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Service>(httpresponse);
                    return true;
                }
                Console.WriteLine("API FAIL ");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in Service AppService=>adding on Content Processing=> " + e.Message);
                Console.WriteLine("Exception in Service AppService=>adding on Content Processing=> Inner=>" + e.InnerException.Message);
                Console.WriteLine("inn=======>" + e.ToString());
                _logger.LogError(e.Message, e);
                return false;
            }
        }

        public async Task<bool> AddServiceCodeOnContentProcessing(Service service)
        {
            try
            {
                var httpservice = _httpClientFactory.CreateClient("ContentProcessingService");
                var result = await httpservice.PostAsync("api/Service/sss", new JsonContent(service));
                if (result.IsSuccessStatusCode)
                {
                    var httpresponse = await result.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Service>(httpresponse);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return false;
            }
        }

        public async Task<List<Service>> SearchService(ServiceSearchDto serviceSearchDto)
        {
            try
            {
                var Services = _aNQContentServiceManageDb.Service.Where(
                    a => a.ServiceId > 0
                    && ((serviceSearchDto.CategoryId == null) || (a.Category == serviceSearchDto.CategoryId))
                    && ((serviceSearchDto.IndustryId == null) || (a.Industry == serviceSearchDto.IndustryId))
                    && ((serviceSearchDto.ServiceProviderId == null) || (a.ServiceProviderId == serviceSearchDto.ServiceProviderId))
                    && ((serviceSearchDto.ServiceCode == null) || (a.ServiceCode.ToLower().Contains(serviceSearchDto.ServiceCode.ToLower())))
                    && a.ApplicationId == serviceSearchDto.ApplicationId
                    ).ToList();

                return Services;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task<List<Service>> GetByShortCodeAndServiceProviderId(string shortCode, long ServiceProviderId)
        {
            try
            {
                //return await _aNQContentServiceManageDb.Service.Where(s => s.ShortCode == shortcode || s.Keyword == keyword);
                return _aNQContentServiceManageDb.Service.Where(s => s.ShortCode == shortCode && s.ServiceProviderId == ServiceProviderId).ToList();

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                return null;
            }
        }

        public async Task<List<ServiceDto>> GetServicesByServiceProviderId(long ServiceProviderId, int skip, int take)
        {
            try
            {
                return await _aNQContentServiceManageDb.Service.Where(s => s.ServiceProviderId == ServiceProviderId)
                    .Select(y => new ServiceDto
                    {
                        ServiceId = y.ServiceId,
                        ServiceName = y.ServiceName,
                        ServiceProviderId = y.ServiceProviderId,
                        ServiceProviderName = y.ServiceProviderName,
                        CreatedOn = y.CreatedOn,
                    }).Skip(skip).Take(take).ToListAsync();

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public int GetServicesByServiceProviderIdCount(long ServiceProviderId)
        {
            try
            {
                return _aNQContentServiceManageDb.Service.Where(s => s.ServiceProviderId == ServiceProviderId).Count();

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return 0;
            }
        }
        public Task<List<ServiceDto>> GetServicesBySPId(long ServiceProviderId)
        {
            try
            {
                return _aNQContentServiceManageDb.Service.Where(s => s.ServiceProviderId == ServiceProviderId)
                    .Select(y => new ServiceDto
                {
                    ServiceId = y.ServiceId,
                    ServiceName = y.ServiceName,
                    ServiceProviderId = y.ServiceProviderId,
                    ServiceProviderName = y.ServiceProviderName,
                    CreatedOn = y.CreatedOn,
                }).ToListAsync();

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task<List<ServiceContentMapDto>> GetServiceContents(long ServiceId)
        {
            try
            {
                return await _aNQContentServiceManageDb.ServiceContentMap.Where(s => s.ServiceId == ServiceId)
                    .Select(y => new ServiceContentMapDto 
                    { 
                        ContentId = y.ContentId
                
                    }).ToListAsync();

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task<bool> GetContentServiceProviderBalance(long serviceProviderId, long ContentId)
        {
            try
            {
            
                var httpservice = _httpClientFactory.CreateClient("WalletService");

                string requestUri = string.Format($"{_walletAppSettings.BaseUrl}" + "/api/Wallet/SpContentWalletBalance/" + $"{serviceProviderId}" + "/" + $"{ContentId}");

                var result = await httpservice.GetAsync(requestUri);
                var httpresponse = await result.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<bool>(httpresponse);
                return data;

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return false;
            }
        }

        public async Task<Service> GetByServiceName(string serviceName)
        {
            return _aNQContentServiceManageDb.Service.FirstOrDefault(a => a.ServiceName == serviceName);
        }



        public async Task SaveAsync()
        {
            await _aNQContentServiceManageDb.SaveChangesAsync();
        }

        #region Helpers
        public async Task<bool> CheckIfNameExist(string name)
        {
            try
            {
                var check =await  _aNQContentServiceManageDb.Service.FirstOrDefaultAsync(x => x.ServiceName.ToUpper() == name.ToUpper());
                if (check != null)
                return true;

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message,e);
                throw;
            }
        }

        public async Task<bool> CheckIfNameExistForContentProvider(long serviceProviderId, string name)
        {
            try
            {
                var check = await _aNQContentServiceManageDb.Service.FirstOrDefaultAsync(x => x.ServiceName.ToUpper() == name.ToUpper()
                                    && x.ServiceProviderId.Equals(serviceProviderId));
                if (check != null)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Name Check falied with an exception for the name: {name}");
                throw;
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _aNQContentServiceManageDb.Database.CloseConnection();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {   
            Dispose(true);
        }
        #endregion
    }

    public class JsonContent : StringContent
    {
        public JsonContent(object obj) :
            base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
        { }
    }
}