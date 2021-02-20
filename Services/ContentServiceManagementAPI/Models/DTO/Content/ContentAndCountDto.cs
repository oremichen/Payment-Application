using ContentManagementServiceAPI.Models.Dto.ContentDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Models.DTO.Content
{
    public class ContentAndCountDto
    {
        public IEnumerable<ContentDto> Contents { get; set; }
        public int Count { get; set; }
    }
}
