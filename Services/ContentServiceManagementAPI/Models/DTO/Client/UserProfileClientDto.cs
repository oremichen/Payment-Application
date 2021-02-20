
namespace ContentServiceManagementAPI.Models.DTO.Client
{
    public class UserProfileClientDto
    {
        public long AuthenticationId { get; set; }

        public string ClientName { get; set; }

        public string ContactPersonFirstName { get; set; }

        public string ContactPersonLastName { get; set; }
        
        public string ContactPersonPhoneNumber { get; set; }

        public string AccountEmail { get; set; }
    }
}
