namespace POS.Application.Common.DTOs
{
    public class WorkflowInstanceDetailsResponse
    {
        public long Id { get; set; }
        public string EntityType { get; set; }
        public long EntityId { get; set; }
        public string CurrentStep { get; set; }
        public long WorkflowStatusId { get; set; }
        public string WorkflowStatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
