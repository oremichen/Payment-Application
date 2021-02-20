using ContentServiceManagementAPI.Models.DTO.Service;
using System;

namespace ContentServiceManagementAPI.Models.DTO.ServiceMappingDto
{
    public class MappedServicesToAClientDto
    {
        public long MappedServiceId { get; set; }
        public long ServiceId { get; set; }

        public ServiceDto Service { get; set; } = null;
        public long ClientId { get; set; }
        public string ServiceProviderName { get; set; }
        public string ServiceName { get; set; }


        public long ServiceProviderId { get; set; }
        
        public long? RateId { get; set; }
        public string RateName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string MappedBy { get; set; }

        public long? DedicatedRateId { get; set; }
        public string DedicatedRateName { get; set; }
        public int RateCurrency { get; set; }
        public string RateCurrencyDescription { get; set; }
        public string RateType { get; set; }
        public bool IsPublished { get; set; }

    }
}
