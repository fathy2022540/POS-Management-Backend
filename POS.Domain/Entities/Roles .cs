namespace JRM.Domain.Entities
{
    public class Roles : BaseEntity
    {
        public string Code { get; set; }
        public string NameEn { get; set; }

        public string? NameAr { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Users>? UsersRoles { get; set; }
        public virtual ICollection<RolesPermissions>? RolesPermissions { get; set; }
        public virtual ICollection<RoleScreenPermissions>? RoleScreenPermissions { get; set; }
    }
}
