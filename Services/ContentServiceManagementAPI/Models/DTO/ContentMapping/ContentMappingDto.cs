using Anq.Enums;
using ContentManagementServiceAPI.Models.Dto.ContentDto;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContentServiceManagementAPI.Models.DTO.ContentMappingDto
{
    //For Update , Delelte and Get Dto Functions
    public class MappedContentDto
    {
        public long MapContentToServiceProviderId { get; set; }
        public long ContentId { get; set; }

        public ContentDto Content  { get; set; }
        public int ContentProviderId { get; set; }
        public long ServiceProviderId { get; set; }
        public ServiceProviderDto ServiceProvider { get; set; }
        public string  ContentProviderName { get; set; }

        // Rate of the content that is mapped.
        public long? RateId { get; set; }    
        public string RateName { get; set; }
        public DateTime CreatedOn { get; set; }

        public string MappedBy { get; set; }
        public long? DedicatedRateId { get; set; }
        public string DedicatedRateName { get; set; }
        public string RateType { get; set; }

        public CurrencyEnum RateCurrency { get; set; }
        public bool IsPublished { get; set; }
        public string ServiceProviderName { get; set; }
    }
    
    public class AddMapContentDto
    {
        [Required]
        public long ContentId { get; set; }
        public long? RateId { get; set; }
        public string RateName { get; set; }

        [Required]
        public long ServiceProviderId { get; set; }
        public string ContentProviderName { get; set; }
        public int ContentProviderId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string MappedBy { get; set; }

        public long? DedicatedRateId { get; set; }
        public string DedicatedRateName { get; set; }
        public string RateType { get; set; }
        public int RateCurrency { get; set; }
        public bool IsPublished { get; set; }


        //New Guys
        public string ContentName { get; set; }
        public string ServiceProviderName { get; set; }
        public string ServiceProviderEmail { get; set; }
        public string ContentProviderEmail { get; set; }
    }

}
