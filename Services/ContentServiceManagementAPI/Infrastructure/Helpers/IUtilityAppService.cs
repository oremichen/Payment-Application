

using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services.Utitlities
{

    // use this to check if an id exist for a table
    public interface IUtilityAppService 
    {
        Task<bool> CheckIfIdExist(long Id);
    }
}
