using ContentServiceManagementAPI.Enums;

namespace ContentServiceManagementAPI.Models
{
    public class ContentProvider
    {
        public ContentProvider()
        {
            Status = EntityStatus.Active;
        }
        public long ContentProviderId { get; set; }

        public long AuthenticationId { get; set; }

        public string ContentProviderName { get; set; }

        public string ContactPersonFirstName { get; set; }

        public string ContactPersonLastName { get; set; }

        public string ContactPersonEmail { get; set; }

        public string ContactPersonPhoneNumber { get; set; }

        public string AccountEmail { get; set; }

        public EntityStatus Status { get; set; }

        public string ApplicationId { get; set; }
    }
}
