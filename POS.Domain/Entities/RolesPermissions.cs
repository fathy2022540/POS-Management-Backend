namespace JRM.Domain.Entities
{
    public class RolesPermissions
    {
        public long RoleId { get; set; }
        public long PermissionId { get; set; }

        public virtual Roles? Role { get; set; }
        public virtual Permissions? Permission { get; set; }
    }
}