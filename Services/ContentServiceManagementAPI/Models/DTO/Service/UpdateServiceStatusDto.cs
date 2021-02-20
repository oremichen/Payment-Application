using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Models.DTO.Service
{
    public class UpdateServiceStatusDto
    {

        public DateTime? ActiveDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long ServiceId { get; set; }
        public int ServiceStatus { get; set; }
    }
}
