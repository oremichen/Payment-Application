using ContentServiceManagementAPI.Enums;
using ContentServiceManagementAPI.Infrastructure.Helpers;

namespace ContentServiceManagementAPI.Models.DTO
{
    public class ContentProviderDto
    {
        private string _phone = "" ;

        public long ContentProviderId { get; set; }

        public long AuthenticationId { get; set; }

        public string ContentProviderName { get; set; }

        public string ContactPersonFirstName { get; set; }

        public string ContactPersonLastName { get; set; }


        public string ContactPersonEmail { get; set; }

        public string AccountEmail { get; set; }

        public string ContactPersonPhoneNumber { get; set; }
           
        public string FormattedContactPersonPhoneNumber
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

        public EntityStatus Status { get; set; }


     
    }
}
