using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class Hotels : BaseEntity
    {
        [Column(TypeName = "nvarchar(300)")]
        public required string Name { get; set; }
        [Column(TypeName = "nvarchar(200)")]    
        public required string LocationCity { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public required string LocationCountry { get; set; }
        public int StarRating { get; set; }

        // Base pricing for reference (actual price might vary by contract)
        public decimal BaseSinglePrice { get; set; }
        public decimal BaseDoublePrice { get; set; }
        public decimal BaseTriplePrice { get; set; }

        // Navigation property: Which journeys use this hotel
        public ICollection<Journey> Journeys { get; set; } = new List<Journey>();

    }
}
