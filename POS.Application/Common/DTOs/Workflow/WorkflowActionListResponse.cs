namespace JRM.Application.Common.DTOs
{
    public class WorkflowActionListResponse
    {
        public long Id { get; set; }
        public long WorkflowInstanceId { get; set; }
        public long UserId { get; set; }
        public long ActionTypeId { get; set; }
        public string ActionTypeName { get; set; }
        public string Comments { get; set; }
    }
}
