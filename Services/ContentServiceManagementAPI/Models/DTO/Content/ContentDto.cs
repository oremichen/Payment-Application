using ContentManagementServiceAPI.Enums;
using ContentServiceManagementAPI.Infrastructure.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContentManagementServiceAPI.Models.Dto.ContentDto
{
    public class ContentDto
    {
        [Key]
        public long ContentId { get; set; }

        public string Name { get; set; }

        public EnumResult ContentPrivacyType { get; set; }
        //public int ContentPrivacyTypeId { get; set; }

        public long CommandRecordId { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        // foreign key to contentOwner

        public int ContentProviderId { get; set; }

        public string FormattedDate { get; set; }

    }

    public class ContentDtos
    {
        [Key]
        public long ContentId { get; set; }

        public string Name { get; set; }

        public ContentPrivacyType ContentPrivacyType { get; set; }

        public long CommandRecordId { get; set; }

        public string Description { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsDeleted { get; set; }

        // foreign key to contentOwner

        public int ContentProviderId { get; set; }

        public string FormattedDate { get; set; }
    }

}
