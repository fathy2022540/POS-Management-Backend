namespace POS.Application.Common.DTOs
{
    public class WorkflowActionDetailsResponse
    {
        public long Id { get; set; }
        public long WorkflowInstanceId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long ActionTypeId { get; set; }
        public string ActionTypeName { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
