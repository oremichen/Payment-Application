using ContentManagementServiceAPI.Enums;
using ContentManagementServiceAPI.Models.Dto.ContentDto;
using ContentServiceManagementAPI.Data;
using ContentServiceManagementAPI.Infrastructure.Helpers;
using ContentServiceManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Services
{
    public class ContentAppService : IContentAppService
    {
        #region Fields
        private ANQContentServiceManageDb _aNQContentServiceManageDb;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ContentAppService> _logger;

        #endregion

        #region Ctro
        public ContentAppService(
            ANQContentServiceManageDb aNQContentServiceManageDb, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<ContentAppService> logger)
        {
            _aNQContentServiceManageDb = aNQContentServiceManageDb;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        #endregion

        #region Action Methods

        public async  Task<long> Add(CreateContentDto entity)
        {
            try
            {
                var content = new Content()
                {
                    ContentProviderId = entity.ContentProviderId,
                    Name = entity.Name,
                    DateCreated = DateTime.Now,
                    CommandRecordId = entity.CommandRecordId,
                    IsDeleted = false,
                    Description = entity.Description,
                    ContentPrivacyType = entity.ContentPrivacyType

                };
                _logger.LogInformation($"{content.ContentProviderId} Added Successfuly");
                await _aNQContentServiceManageDb.Content.AddAsync(content);
                await Save();
                return content.ContentId;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public async Task Delete(ContentDto entity)
        {
            try
            {
                _logger.LogInformation($"Deleting Client");
                var content = await _aNQContentServiceManageDb.Content.FindAsync(entity.ContentId);
                _aNQContentServiceManageDb.Content.Remove(content);
                await this.Save();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public async Task<ContentDto> GetById(long id)
        {
            try
            {
                var content = await _aNQContentServiceManageDb.Content.FindAsync(id);
                if (content != null)
                {
                    var contentDto = new ContentDto()
                    {
                        ContentId = content.ContentId,
                        ContentProviderId = content.ContentProviderId,
                        CommandRecordId = content.CommandRecordId,
                        ContentPrivacyType = EnumHelper.GetEnumResultByEnumObject<ContentPrivacyType>(content.ContentPrivacyType),
                        Description = content.Description,
                        Name = content.Name,
                        IsDeleted = false,
                        DateCreated = DateTime.UtcNow,
                    };
                    _logger.LogInformation($"{contentDto.ContentId} returned successfully");
                    return contentDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger
                    .LogError(e.Message, e);
                throw e;
            }
            
        }

        public async Task<Content> GetContentById(long id)
        {
            try
            {
                var content = await _aNQContentServiceManageDb.Content.FindAsync(id);
                if (content != null)
                {
                    var contentDto = new Content()
                    {
                        ContentId = content.ContentId,
                        ContentProviderId = content.ContentProviderId,
                        CommandRecordId = content.CommandRecordId,
                       // ContentPrivacyType = EnumHelper.GetEnumResultByEnumObject<ContentPrivacyType>(content.ContentPrivacyType),
                        Description = content.Description,
                        Name = content.Name,
                        IsDeleted = false,
                        DateCreated = DateTime.UtcNow,
                    };                 
                    return contentDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger
                    .LogError(e.Message, e);
                throw e;
            }

        }




        public async Task<IEnumerable<ContentDto>> GetAll()
        {
            try
            {
                var contentsDto = new List<ContentDto>();
                var contents = await _aNQContentServiceManageDb.Content.ToListAsync();

                contentsDto.AddRange(contents.OrderByDescending(x => x.DateCreated).Select(content => new ContentDto()
                {
                    ContentId = content.ContentId,
                    ContentProviderId = content.ContentProviderId,
                    CommandRecordId = content.CommandRecordId,
                    ContentPrivacyType = EnumHelper.GetEnumResultByEnumObject<ContentPrivacyType>(content.ContentPrivacyType),
                    Description = content.Description,
                    Name = content.Name,
                    IsDeleted = false,
                    DateCreated = content.DateCreated,
                    FormattedDate = content.DateCreated.ToShortDateString()
                }));

                _logger.LogInformation($"Success!{contentsDto.Count()} contentts returned");


                return contentsDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
             
           
        }

        public async Task<IEnumerable<ContentDto>> GetContentsByContentProvider(long contentProviderId)
        {
            try
            {
                var contentsDto = new List<ContentDto>();

                var contents = await _aNQContentServiceManageDb.Content.Where(c => c.ContentProviderId == contentProviderId).ToListAsync();

                contentsDto.AddRange(contents.OrderByDescending(x => x.DateCreated).Select(content => new ContentDto()
                {
                    ContentId = content.ContentId,
                    ContentProviderId = content.ContentProviderId,
                    CommandRecordId = content.CommandRecordId,
                    ContentPrivacyType = EnumHelper.GetEnumResultByEnumObject<ContentPrivacyType>(content.ContentPrivacyType),
                    Description = content.Description,
                    Name = content.Name,
                    IsDeleted = false,
                    DateCreated = content.DateCreated,
                    FormattedDate = content.DateCreated.ToShortDateString()
                }));

                return contentsDto;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
           
        }

        public async Task<IEnumerable<ContentDto>> SearchByIntegerColumn(long column)
        {
            try
            {
                var contents = await _aNQContentServiceManageDb.Content.
               AsNoTracking().Where(x => x.ContentId == column).ToListAsync();
                if (contents.Count() > 0)
                {
                    var contentsDto = new List<ContentDto>();
                    contentsDto.AddRange(contents.OrderByDescending(x => x.ContentId).Select(content => new ContentDto()
                    {
                        ContentId = content.ContentId,
                        ContentProviderId = content.ContentProviderId,
                        CommandRecordId = content.CommandRecordId,
                        ContentPrivacyType = EnumHelper.GetEnumResultByEnumObject<ContentPrivacyType>(content.ContentPrivacyType),
                        Description = content.Description,
                        Name = content.Name,
                        IsDeleted = false,
                        DateCreated = content.DateCreated,
                        FormattedDate = content.DateCreated.ToShortDateString()
                    }));
                    return contentsDto;
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public async Task<long> Update(ContentDto model)
        {
            try
            {       
                var content = await _aNQContentServiceManageDb.Content.FindAsync(model.ContentId);

                content.ContentId = model.ContentId;
                content.ContentProviderId = model.ContentProviderId;
                content.CommandRecordId = model.CommandRecordId;
               // content.ContentPrivacyType = model.ContentPrivacyType;
                content.DateCreated = model.DateCreated;
                content.Description = model.Description;
                content.Name = model.Name;
                content.IsDeleted = false;
            
                 _aNQContentServiceManageDb.Entry(content).State = EntityState.Modified;
                await this.Save();

                return content.ContentId;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }

        public static Content ContentHelper(ContentDto content)
        {
            Content eContent = new Content();

            eContent.ContentProviderId = content.ContentProviderId;
            eContent.CommandRecordId = content.CommandRecordId;
            //eContent.ContentPrivacyType = content.ContentPrivacyType.Id;
            eContent.DateCreated = content.DateCreated;
            eContent.Description = content.Description;
            eContent.Name = content.Name;
            eContent.IsDeleted = false;

            return eContent;
        }



        public async Task Save()
        {
            await _aNQContentServiceManageDb.SaveChangesAsync();
        }

        #endregion

        #region Helpers

        // Searches if name exist for a ContentProvider
        public async Task<bool> CheckIfNameExistForContentProvider(long contentProviderId, string name)
        {
            try
            {
                var checkContents = await _aNQContentServiceManageDb.Content.Where(x => x.Name == name && x.ContentProviderId == contentProviderId)
                                                                           .FirstOrDefaultAsync();
                if (checkContents != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw;
            }
               
        }
      
        public async Task<bool> CheckIfNameExist(string name)
        {
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    var checkContent = await _aNQContentServiceManageDb.Content.FirstOrDefaultAsync(x => x.Name == name);
                    if (checkContent != null)
                    {
                        if (checkContent.Name.ToUpper() == name.ToUpper())
                        {
                            return true;
                        }
                        return false;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                throw e;
            }
           
        }
        #endregion

    }
}
