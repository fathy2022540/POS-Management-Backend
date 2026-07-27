namespace JRM.Application.Common.DTOs
{
    public class RolesModel
    {

        public long? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; } = true;

    }

}
