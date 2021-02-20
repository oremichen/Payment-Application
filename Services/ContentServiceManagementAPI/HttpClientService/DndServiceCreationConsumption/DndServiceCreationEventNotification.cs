using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContentServiceManagementAPI.HttpClientService.DndServiceCreationConsumption
{
    public class DndServiceCreationEventNotification
    {
        public async Task Notify(dynamic payload)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = payload;
            }
        }
    }
}
