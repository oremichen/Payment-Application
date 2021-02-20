using Anq.Data;
using ContentServiceManagementAPI.Models.DTO.Service;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.DapperImplementation
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionStrings _appSettings;
        public ServiceRepository(IConfiguration configuration, IOptions<ConnectionStrings> options)
        {
            _configuration = configuration;
            _appSettings = options.Value;
        }
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_appSettings.ANQContentServiceManageDbConnection);
            }
        }

        public async Task<List<ServiceDto>> GetServiceWithSkipTakAndPagination(long spId, string keyword = null, int skip = 0, int take = int.MaxValue)
        {
            using (var conn = Connection)
            {
                var sql = $"dbo.spGetAllservicesByServiceProviderIdAndTakeSkipSearch @spId, @skip, @take, @keyword";
                var result = await conn.QueryAsync<ServiceDto>(sql, new { spId, skip, take, keyword });
                return result.ToList();
            }
        }

        public async Task<long> GetServiceWithCountAndPagination(long spId, string keyword = null)
        {
            using (var conn = Connection)
            {
                var sql = $"dbo.spGetAllservicesByServiceProviderIdSearchAndCount  @spId,@keyword";
                var result = await conn.ExecuteScalarAsync<long>(sql, new { spId, keyword });
                return result;
            }
        }
    }
}
