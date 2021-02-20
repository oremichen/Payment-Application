

namespace ContentServiceManagementAPI.Models.DTO.ServiceProvider
{
    public class DndSpUpdateRequest
    {
        public long UserId { get; set; }
        public long ServiceProviderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}