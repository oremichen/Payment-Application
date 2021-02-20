
using System.ComponentModel.DataAnnotations;

namespace ContentServiceManagementAPI.Models.DomainModels
{
    public class CommandRecord
    {
        [Key]
        public long Id { get; set;}
        public long CommandRecordId { get; set; }
        public string Type { get; set; }
        public string Group { get; set; }
        public string SystemName { get; set; }
        public string Description { get; set; }
        public long ClientId { get; set; }
    }
}
