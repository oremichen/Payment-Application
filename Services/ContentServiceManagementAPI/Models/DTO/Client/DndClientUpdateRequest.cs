using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Models.DTO.Client
{
    public class DndClientUpdateRequest
    {
        public long UserId { get; set; }
        public long ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
    }
}
