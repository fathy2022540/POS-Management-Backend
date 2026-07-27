namespace JRM.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public long UserId { get; set; }

        public string EntityName { get; set; }

        public long EntityId { get; set; }

        public string ActionType { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }
    }
}

