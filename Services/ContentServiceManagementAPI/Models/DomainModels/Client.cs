using ContentServiceManagementAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace ContentServiceManagementAPI.Models.DomainModels
{
    public class Client
    {
        public Client()
        {
            Status = EntityStatus.Active;
        }

        [Key]
        public long ClientId { get; set; }

        public long AuthenticationId { get; set; }

        public string ClientName { get; set; }

        public string ContactPersonFirstName { get; set; }

        public string ContactPersonLastName { get; set; }

        public string ContactPersonEmail { get; set; }

        public string ContactPersonPhoneNumber { get; set; }

        public string AccountEmail { get; set; }

       public EntityStatus Status { get; set; }

        public string ApplicationId { get; set; }
        //public string CountryName { get; set; }
        //public string CountryCode { get; set; }
        //public string AddressLocation { get; set; }

    }
}
