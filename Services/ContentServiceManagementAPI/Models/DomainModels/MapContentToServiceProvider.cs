using Anq.Enums;
using System;

namespace ContentServiceManagementAPI.Models
{
    public class MapContentToServiceProvider
    {
        public long MapContentToServiceProviderId { get; set; }
        public int ContentProviderId { get; set; }
        public string ContentProviderName { get; set; }

        public long  ContentId { get; set; }
        public long ServiceProviderId { get; set; }
        public DateTime CreatedOn { get; set; }

        public string MappedBy { get; set; }
        // for the content being mapped
        public long? RateId { get; set; }
        public string RateName { get; set; }
        public long? DedicatedRateId { get; set; }

        public string RateType { get; set; }
        public string DedicatedRateName { get; set; }

        public CurrencyEnum RateCurrency { get; set; }

        public bool IsPublished { get; set; }
        //public string ServiceProviderName { get; set; }
    }
}
