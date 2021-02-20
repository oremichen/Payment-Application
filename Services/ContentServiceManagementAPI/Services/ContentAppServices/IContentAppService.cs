using ContentManagementServiceAPI.Models.Dto.ContentDto;
using ContentServiceManagementAPI.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services
{
    public interface IContentAppService 
    {

        Task<IEnumerable<ContentDto>> GetContentsByContentProvider(long contentProviderId);
        Task<IEnumerable<ContentDto>> GetAll();

        Task<long> Add(CreateContentDto entity);

        Task<long> Update(ContentDto entity);

        Task<ContentDto> GetById(long id);

        Task Delete(ContentDto entity);
       
        // Validations
        Task<bool> CheckIfNameExistForContentProvider(long contentProviderId , string name);
        Task<bool> CheckIfNameExist(string name);

    }
}
