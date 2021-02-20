using ContentServiceManagementAPI.Enums;
using ContentServiceManagementAPI.Infrastructure.Helpers;
using System;

namespace ContentServiceManagementAPI.Models.DTO
{
    public class ServiceProviderDto
    {
        private string _contactPersonPhone = "";

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

        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string AddressLocation { get; set; }
        public long CountryId { get; set; }


        public string FormmatedContactPersonPhoneNumber
        {
           
            get {
                if (string.IsNullOrEmpty(ContactPersonPhoneNumber))
                {
                    return null;
                }
                return TelephoneHelper.FormatPhone(ContactPersonPhoneNumber);
            }
        }


    }

}
