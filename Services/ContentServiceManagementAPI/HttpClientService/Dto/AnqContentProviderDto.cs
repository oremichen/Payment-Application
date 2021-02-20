

namespace ContentServiceManagementAPI.HttpClientService.Dto
{
    public class AnqContentProviderDto
    {
        public string AuthenticationId { get; set; }

        public string ClientName { get; set; }

        public string ContactPersonFirstName { get; set; }

        public string ContactPersonLastName { get; set; }

        public string ContactPersonEmail { get; set; }

        public string ContactPersonPhoneNumber { get; set; }

        public bool IsProcessApi { get; set; }

        public bool IsProcessFtp { get; set; }

        public bool IsProcessWeb { get; set; }

        public bool IsContentProvider { get; set; }

        public string ClientAccountEmail { get; set; }

        public int ClientId { get; set; }
        public int Currency { get; set; }
    }
}

