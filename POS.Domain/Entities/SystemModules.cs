using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Domain.Entities
{
    public class SystemModules : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Name { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? NameAR { get; set; }

        public int SortOrder { get; set; }

        public virtual ICollection<SystemScreens>? Screens { get; set; }
    }
}