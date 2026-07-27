using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public long UserId { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string TitleEn { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(250)")]
        public string TitleAr { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(1000)")]
        public string MessageEn { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(1000)")]
        public string MessageAr { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(100)")]
        public string? ReferenceType { get; set; }

        public long? ReferenceId { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string? ActionUrl { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}