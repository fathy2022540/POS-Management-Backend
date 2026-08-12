using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POS.Domain.Entities
{
    public partial class ParamteresConfiguration : BaseEntity
    {
        [Required]
        [Column(TypeName = "nvarchar(400)")]
        public string Keyword { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string? Parent { get; set; }

        [Column(TypeName = "nvarchar(Max)")]
        public string? DescriptionEn { get; set; }
        [Column(TypeName = "nvarchar(Max)")]
        public string? DescriptionAr { get; set; }

        [Column(TypeName = "nvarchar(Max)")]
        public string? ContentEn { get; set; }
        [Column(TypeName = "nvarchar(Max)")]
        public string? ContentAr { get; set; }
        [Column(TypeName = "nvarchar(Max)")]
        public string? URL { get; set; }

    }
}
