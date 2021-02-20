using System;

namespace ContentServiceManagementAPI.Models
{
    public class MapServiceToClient
    {
        public long MapServiceToClientId { get; set; }

        public long ClientId { get; set; }

        //TODO Added to get a folder with the name
        public string ClientName { get; set; }

        //TODO Verify it it can be allowed 
        public long ClientAuthId { get; set; }

        public long ServiceId { get; set; }
        public string ServiceName { get; set;}

        public long ServiceProviderId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string MappedBy { get; set; }
        public string ServiceProviderName { get; set; }


        public long RateId { get; set; }
        public string RateName { get; set; }

        public long? DedicatedRateId { get; set; }
        public string DedicatedRateName { get; set; }

        public string RateType { get; set; }

        public int RateCurrency { get; set; }

        public bool IsPublished { get; set; }
    }
}
