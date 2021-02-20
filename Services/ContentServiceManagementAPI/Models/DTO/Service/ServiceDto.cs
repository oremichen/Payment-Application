using ContentServiceManagementAPI.Controllers;
using ContentServiceManagementAPI.Models.DomainModels;
using System;
using System.Collections.Generic;

namespace ContentServiceManagementAPI.Models.DTO.Service
{
    public class ServiceAndContentDetails : ServiceDto
    {
        public IEnumerable<ServiceContentMapp> ServiceContentMaps { get; set; }
    }


    public class ServiceDto
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
       
        public string Note { get; set; }

        public DateTime CreatedOn { get; set; }
        public bool Active { get; set; }

        public string FormattedCreatedOnDate { get; set; }

        // foreign key
        public long ServiceProviderId { get; set; }
        public string ServiceProviderName { get; set; }
        public string ServiceCode { get; set; }
        public decimal? Price { get; set; }
        public string ShortCode { get; set; }
        public string Keyword { get; set; }


        public int? SubscriptionType { get; set; }
        public int? Periodicity { get; set; }
        public long? Frequency { get; set; }
        public int? ServiceStatus { get; set; }

        public string Channel { get; set; }
        public int? Industry { get; set; }
        public int? Category { get; set; }
        public string CategoryCode { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ActiveDate { get; set; }

        public long ApplicationId { get; set; }
        public string Application { get; set; }
        public string OperatorIds { get; set; }

        public int? BillBasisId { get; set; }
        public long? BillPerTransactionCount { get; set; }
        public long? MobileOriginatingCount { get; set; }
        public long? MobileTerminatingCount { get; set; }
        public bool? IsVasSystemService { get; set; }
        public string VasSystemServiceCode { get; set; }
      //  public ServiceCategoryCodeEnum ServiceCategoryCode { get; set; }
    }  

    public class serviceCountDto
    {
        public List<ServiceDto> Services { get; set; }
        public int Count { get; set; }
    }
}