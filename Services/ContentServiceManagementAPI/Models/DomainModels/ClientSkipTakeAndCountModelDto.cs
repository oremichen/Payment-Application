
using System.Collections.Generic;

namespace ContentServiceManagementAPI.Models.DomainModels
{
    public class ClientSkipTakeAndCountModelDto
    {
        public IEnumerable<Client> ClientModel { get; set; }

        public long CountClients { get; set; }

    }
}
