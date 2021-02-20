using ContentServiceManagementAPI.Models.DTO.Client;
using System;

namespace ContentServiceManagementAPI.Models.DTO.ServiceMappingDto
{
    public class MappedClientsToAServiceDto
    {
        public long MappedServiceId { get; set; }
        public long ServiceId { get; set; }
        public long ClientId { get; set; }
        public ClientDto Client { get; set; }
        public string ServiceName { get; set; }

        public long ServiceProviderId { get; set; }

        public long? RateId { get; set; }
        public string RateName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string MappedBy { get; set; }

        public long? DedicatedRateId { get; set; }
        public string DedicatedRateName { get; set; }
        public string RateType { get; set; }
        public int RateCurrency { get; set; }
        public bool IsPublished { get; set; }
    }
}
