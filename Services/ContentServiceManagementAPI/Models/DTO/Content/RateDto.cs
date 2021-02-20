
using System;

namespace ContentServiceManagementAPI.Models.DTO.Content
{
    public class RateDto
    {
        public long RateId { get; set; }

        public long? ContentId { get; set; }
        public long ContentProviderId { get; set; }

        public int Currency { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Name { get; set; }

        public DateTime EffectiveDate { get; set; }
        public DateTime TerminationDate { get; set; }

        public bool IsDedicated { get; set; }
        public bool IsDeleted { get; set; }

    }
}
