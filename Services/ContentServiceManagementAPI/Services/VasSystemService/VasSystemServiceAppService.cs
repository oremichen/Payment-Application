using ContentServiceManagementAPI.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.VasSystemService
{
    public class VasSystemServiceAppService : IVasSystemServiceAppService
    {
        private readonly ANQContentServiceManageDb _aNQContentServiceManageDb;
        private readonly ILogger<ANQContentServiceManageDb> logger;
        public VasSystemServiceAppService(
            ANQContentServiceManageDb aNQContentServiceManageDb,
            ILogger<ANQContentServiceManageDb> logger
            )
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            this.logger = logger;
        }
        public async Task<List<Models.DomainModels.VasSystemService>> GetAll()
        {
            try
            {
                var services = _aNQContentServiceManageDb.VasSystemServices.ToList();
                return services;
            }
            catch (Exception e)
            {
                logger.LogError(e.Message, e);
                throw e;
            }
            
        }
    }
}
