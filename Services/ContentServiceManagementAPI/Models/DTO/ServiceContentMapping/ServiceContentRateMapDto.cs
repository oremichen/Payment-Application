using Anq.Enums;
using ContentServiceManagementAPI.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Models.DTO.ServiceContentMapping
{
    public class ServiceContentRateMapDto
    {
        public ServiceRateDtos[] ServiceRate { get; set; }
        public CreateServiceContentMapDto[] ServiceContentMap { get; set; }
    }


    public class ServiceRateDtos
    {
        public long ServiceRateId { get; set; }
        public string Name { get; set; }
        public long? ServiceId { get; set; }
        public long ServiceProviderId { get; set; }

        public int Currency { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime EffectiveDate { get; set; }
        public DateTime TerminationDate { get; set; }

        public bool IsDedicated { get; set; }
        public bool IsDeleted { get; set; }
    }
}
