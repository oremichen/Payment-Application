using ContentServiceManagementAPI.Models.DTO.Service;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.DapperImplementation
{
    public interface IServiceRepository
    {
        IDbConnection Connection { get; }

        Task<List<ServiceDto>> GetServiceWithSkipTakAndPagination(long spId, string keyword = null, int skip = 0, int take = int.MaxValue);
        Task<long> GetServiceWithCountAndPagination(long spId, string keyword = null);
    }
}