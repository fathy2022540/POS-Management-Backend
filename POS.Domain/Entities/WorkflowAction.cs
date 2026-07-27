using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class WorkflowAction : BaseEntity
    {
        public long UserId { get; set; }

        public string Comments { get; set; }

        public long ActionTypeId { get; set; }

        public long WorkflowInstanceId { get; set; }

        [ForeignKey("ActionTypeId")]
        public LookupItems ActionType { get; set; }

        [ForeignKey("UserId")]
        public Users User { get; set; }

        [ForeignKey("WorkflowInstanceId")]
        public WorkflowInstance WorkflowInstance { get; set; }
    }
}
