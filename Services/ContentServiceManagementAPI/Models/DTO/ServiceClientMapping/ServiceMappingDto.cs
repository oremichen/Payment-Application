using Anq.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContentServiceManagementAPI.Models.DTO.ServiceMappingDto
{
    //For Update , Delelte and Get Dto Functions
    public class MappedServiceDto
    {
        public long MappedServiceId { get; set; }

        public long ServiceId { get; set; }
 
        public long ClientId { get; set; }


        //TODO Added to get a folder with the name
        public string ClientName { get; set; }

        //TODO Verify it it can be allowed 
        public long ClientAuthId { get; set; }

        public string ServiceProviderName { get; set; }
        public long ServiceProviderId { get; set; }
        public string ServiceName { get; set; }

        public long RateId { get; set; }
        public string RateName { get; set; }
        public DateTime CreatedOn { get; set; } 
        public string MappedBy { get; set; }

        public long? DedicatedRateId { get; set; }
        public string DedicatedRateName { get; set; }
        public string RateType { get; set; }
        public CurrencyEnum RateCurrency { get; set; }
        public bool IsPublished { get; set; }
    }
  
    //For Create Dto function
    public class AddMapServiceDto
    {
        [Required]
        public long ServiceId { get; set; }
        [Required]
        public long ServiceProviderId { get; set; }
        [Required]
        public int ClientId { get; set; }


        //TODO Added to get a folder with the name
        public string ClientName { get; set; }

        //TODO Verify it it can be allowed 
        public long ClientAuthId { get; set; }

        public string ServiceProviderName { get; set; }
        public string ServiceName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string MappedBy { get; set; }
        public long RateId { get; set; } = 0;
        public string RateName { get; set; }

        public long? DedicatedRateId { get; set; }
        public string DedicatedRateName { get; set; }
        public string RateType { get; set; }
        public CurrencyEnum RateCurrency { get; set; }
        public bool IsPublished { get; set; }



        //New guys
        public string ClientEmailAddress { get; set; }
        public string ServiceProviderEmail { get; set; }
    }
}
