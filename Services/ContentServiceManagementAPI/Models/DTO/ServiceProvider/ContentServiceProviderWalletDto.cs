using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.Models.DTO.ServiceProvider
{
    public class ContentServiceProviderWalletDto
    {
        public int ContentId { get; set; }
        public int ServiceProviderId { get; set; }
        public int WalletId { get; set; }
        public int CurrentBalance { get; set; }
        public int Currency { get; set; }
        public int Status { get; set; }
        public DateTime LastBilledDate { get; set; }
    }

    public class ContentSpDto
    {
        public long ServiceProviderId { get; set; }
        public long ContentId { get; set; }
    }
}
