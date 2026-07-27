using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class SystemScreens : BaseEntity
    {
        public long ModuleId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string ScreenCode { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string ScreenName { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? ScreenNameAR { get; set; }

        public int SortOrder { get; set; }

        [ForeignKey("ModuleId")]
        public virtual SystemModules? Module { get; set; }

        public virtual ICollection<RoleScreenPermissions>? RoleScreenPermissions { get; set; }
        public virtual ICollection<UserScreenPermissionOverrides>? UserScreenPermissionOverrides { get; set; }
    }
}