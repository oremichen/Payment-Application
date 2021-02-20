using System;
using System.ComponentModel.DataAnnotations;

namespace ContentServiceManagementAPI.Models.DomainModels
{
    public class ServiceContentMapp
    {
        [Key]
        public long ServiceContentMappId { get; set; }

        public long ServiceId { get; set; }

        public long ContentId { get; set; }

        public DateTime CreatedOn { get; set; } 

        public long ServiceProviderId { get; set; }

        // Added March 18 2020 by 11 am by chris.
        // This i neccessary such that basic details as name should be available here
        // rather having to call another table to get just a name of content or service with the respective Id(s)
        
        
        //public string ContentName { get; set; }

        //public string ServiceName { get; set; }

    }
}
