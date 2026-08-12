using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Domain.Entities
{
    public class LookupItems : BaseEntity
    {
        [Required]

        public long LookupId { get; set; }
        public long? LookupItemParentId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(400)")]
        public string NameEn { get; set; }

        [Column(TypeName = "nvarchar(400)")]
        public string? NameAR { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Code { get; set; }

        [ForeignKey("LookupId")]
        public virtual Lookup Lookup { get; set; }

        public virtual ICollection<WorkflowAction> WorkflowActions { get; set; }
        public virtual ICollection<WorkflowInstance> WorkflowInstances { get; set; }

    }
}
