
namespace ContentServiceManagementAPI.Models.DTO.Service
{
    public class ServiceSearchDto
    {
        public int? IndustryId { get; set; }
        public int? CategoryId { get; set; }
        public int? ServiceProviderId { get; set; }
        public string ServiceCode { get; set; }
        public int ApplicationId { get; set; }
    }
}