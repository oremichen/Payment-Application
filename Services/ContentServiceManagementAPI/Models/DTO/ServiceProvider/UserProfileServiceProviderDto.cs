
namespace ContentServiceManagementAPI.Models.DTO.ServiceProvider
{
    public class UserProfileServiceProviderDto
    {
        public long ServiceProviderId { get; set; }
        public string ServiceProviderName { get; set; }
	    public string AccountEmail { get; set; }
	    public string ContactPersonFirstName { get; set; }
	    public string ContactPersonLastName { get; set; }
	    public string ContactPersonPhoneNumber { get; set; }
    }
}
