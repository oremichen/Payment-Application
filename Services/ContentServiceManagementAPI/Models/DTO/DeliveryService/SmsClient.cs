
using System;

namespace ContentServiceManagementAPI.Models.DTO.DeliveryService
{
    public class SmsClient
    {
        /// </summary>
        public long AnqClientId { get; set; }
        public long AnqUserId { get; set; }
        public string CreatorId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string ApiKey { get; set; }
        public int EntityStatus { get; set; }
        public DateTime DateCreated { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string AddressLocation { get; set; }
    }
}
