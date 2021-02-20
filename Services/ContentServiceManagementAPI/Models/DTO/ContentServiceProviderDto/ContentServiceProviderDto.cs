using Anq.Enums;
using System;

namespace ContentServiceManagementAPI.Models.DTO.ContentServiceProviderDto
{
    public class ContentServiceProviderDto
    {
        public long ContentProviderId { get; set; }
        public long ServiceProviderId { get; set; }
        public string ServiceProviderName { get; set; }
        public string AccountEmail { get; set; }
        public long AuthenticationId { get; set; }
        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ApplicationId { get; set; }
        public EntityStatus Status { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public string VasLicenseId { get; set; }
        public DateTime? VasLicenseActiveDate { get; set; }
        public DateTime? VasLicenseExpiryDate { get; set; }
        public string Approved { get; set; }
        public string DeclineReason { get; set; }
    }
}
