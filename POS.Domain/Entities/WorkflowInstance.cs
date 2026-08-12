using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Domain.Entities
{
    public class WorkflowInstance : BaseEntity
    {
        public string EntityType { get; set; }

        public long EntityId { get; set; }

        public string CurrentStep { get; set; }

        public long WorkflowStatusId { get; set; }

        [ForeignKey("WorkflowStatusId")]

        public virtual LookupItems WorkflowStatus { get; set; }

        public virtual ICollection<WorkflowAction> Actions { get; set; }
    }





}

