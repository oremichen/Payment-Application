using ContentManagementServiceAPI.Models.Dto.ContentDto;
using System;

namespace ContentServiceManagementAPI.Models.DTO.ServiceContentMapping
{
    public class ServiceContentMapDto
    {         
       public long ServiceContentMapId { get; set; }
       public long ServiceId { get; set; }
       public long ContentId { get; set; }
        public ContentDto Content { get; set; }
        public string  ContentProviderName { get; set; }
        public DateTime CreatedOn { get; set; }
        public long ServiceProviderId { get; set; }

    }
    public class CreateServiceContentMapDto
    {
        public long ServiceId { get; set; }
        public long ContentId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public long ServiceProviderId { get; set; }
        public long MapContentToServiceProviderId { get; set; }
    }

    public class ServiceContentMapServiceNameDto
    {
        public long ServiceContentMapId { get; set; }
        public long ServiceId { get; set; }
        public long ContentId { get; set; }
        public ContentDto Content { get; set; }
        public string ContentProviderName { get; set; }
        public string ServiceName { get; set; }
        public string ContentName { get; set; }
        public DateTime CreatedOn { get; set; }
        public long ServiceProviderId { get; set; }

    }

    //public class ServiceRateMapsDtos
    //{
    //    public long ServiceRateId { get; set; }
    //    public string Name { get; set; }
    //    public long? ServiceId { get; set; }
    //    public long ServiceProviderId { get; set; }

    //    public EnumResult Currency { get; set; }
    //    public DateTime CreatedOn { get; set; }

    //    public DateTime EffectiveDate { get; set; }
    //    public DateTime TerminationDate { get; set; }

    //    public bool IsDedicated { get; set; }
    //    public bool IsDeleted { get; set; }
    //}
}
