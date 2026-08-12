namespace POS.Domain.Entities
{
    public class BaseAuditableEntity
    {
        public Guid TenantId { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool Is_Deleted { get; set; } = false;
    }
}
