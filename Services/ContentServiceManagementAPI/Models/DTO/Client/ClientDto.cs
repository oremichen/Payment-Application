using ContentServiceManagementAPI.Enums;
using ContentServiceManagementAPI.Infrastructure.Helpers;

namespace ContentServiceManagementAPI.Models.DTO.Client
{
    public class ClientDto
    {
        public long ClientId { get; set; }

        private string _phone = "";

        public long AuthenticationId { get; set; }

        public string ClientName { get; set; }

        public string ContactPersonFirstName { get; set; }

        public string ContactPersonLastName { get; set; }

        public string ContactPersonEmail { get; set; }

        public string ContactPersonPhoneNumber{get; set;}

        public string AccountEmail { get; set; }

        public EntityStatus Status { get; set; }

        public string ApplicationId { get; set; }

        public string FormattedPhoneNumber
        {
            get
            {
                if (string.IsNullOrEmpty(ContactPersonPhoneNumber))
                {
                    return null;
                }
                return TelephoneHelper.FormatPhone(ContactPersonPhoneNumber);
            }
        }

    
    }
}
