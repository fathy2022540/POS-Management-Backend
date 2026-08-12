using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Domain.Entities
{
    public class Permissions : BaseEntity
    {
        public string? Code { get; set; }

        [Column(TypeName = "nvarchar(150)")]

        public string Name { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string? Resource { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? Action { get; set; }

        public virtual ICollection<RolesPermissions>? RolePermissions { get; set; }
    }
}
