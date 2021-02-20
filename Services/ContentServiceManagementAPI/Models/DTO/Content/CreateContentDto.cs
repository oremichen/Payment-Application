using ContentManagementServiceAPI.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace ContentManagementServiceAPI.Models.Dto.ContentDto
{
    public class CreateContentDto
    {
        [Required]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Not More than 200 letters sentence")]
        public string Description { get; set; }

        [Display(Name = "Command Record")]
        public long CommandRecordId { get; set; }

        public ContentPrivacyType ContentPrivacyType { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        //foriegn keys

        public int ContentProviderId { get; set; }

    }
}
