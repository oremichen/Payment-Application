
namespace ContentServiceManagementAPI.HttpClientService.Dto
{
    public class AnqPartnerDto
    {
        public long ServiceProviderId { get; set; }
        public int PartnerId { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddress { get; set; }
        public string PartnerPhoneNumber { get; set; }
        public int AuthenticationId { get; set; }
        public string ContactPersonFirstName { get; set; }
        public string ContactPersonLastName { get; set; }
        public string ContactPersonEmail { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public string BusinessEmail { get; set; }
        public string SupportEmail { get; set; }
        public bool IsDeleted { get; set; }
    }
}
