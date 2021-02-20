using ContentManagementServiceAPI.Enums;
using System;

namespace ContentServiceManagementAPI.Models
{
    public class Content
    {
        public long ContentId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long CommandRecordId { get; set; }

        public ContentPrivacyType ContentPrivacyType { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime DateCreated { get; set; }

        // foreign keys
        public int ContentProviderId { get; set; }

        public long ApplicationId { get; set; }


    }
}
