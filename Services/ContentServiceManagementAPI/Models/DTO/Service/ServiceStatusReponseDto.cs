using System;

namespace ContentServiceManagementAPI.Models.DTO.Service
{
    public class ServiceStatusReponseDto
    {
        public bool Successful { get; set; }
        public string Message { get; set; }
        public long ServiceId { get; set; }
        public int PreviousServiceStatus { get; set; }
        public int ServiceStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
