using ContentManagementServiceAPI.Models.Dto.ContentDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Models.DTO.ContentProvider
{
    public class ContentProviderAndContentDto
    {
        public ContentProviderDto ContentProvider { get; set; }
        public IEnumerable<ContentDto> contents { get; set; }
    }
}
