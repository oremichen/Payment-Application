using ContentServiceManagementAPI.Models.DTO.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Models.DTO.ServiceProvider
{
    public class ServiceProviderServicesCount
    {
        public long Count { get; set; }
        public IEnumerable<ServiceDto> SpServices { get; set; }
        public string  Search { get; set; }
    }
}
