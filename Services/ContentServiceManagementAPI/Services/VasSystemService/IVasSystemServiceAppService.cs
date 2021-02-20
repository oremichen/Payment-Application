using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.VasSystemService
{
    public interface IVasSystemServiceAppService
    {
        Task<List<Models.DomainModels.VasSystemService>> GetAll();
    }
}
