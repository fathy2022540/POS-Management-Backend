namespace JRM.Application.Common.DTOs
{
    public class WorkflowInstanceListResponse
    {
        public long Id { get; set; }
        public string EntityType { get; set; }
        public long EntityId { get; set; }
        public string CurrentStep { get; set; }
        public long WorkflowStatusId { get; set; }
        public string WorkflowStatusName { get; set; }
    }
}
