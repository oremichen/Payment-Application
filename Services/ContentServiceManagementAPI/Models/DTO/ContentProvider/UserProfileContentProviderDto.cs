
namespace ContentServiceManagementAPI.Models.DTO.ContentProvider
{
    public class UserProfileContentProviderDto
    {
        public long ContentProviderId { get; set; }
        public string ContentProviderName { get; set; }
        public string AccountEmail { get; set; }
        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
    }
}
