using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class Company : BaseEntity
    {
        [Column(TypeName = "nvarchar(200)")]
        public required string Name { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public required string NameAR{ get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public required string ContactEmail { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string? ContactPhone { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string? ContactEmployee { get; set; }

        public string? Note { get; set; }

        // Navigation properties
        public ICollection<Journey> Journeys { get; set; } = new List<Journey>();
        public ICollection<Bus> Buses { get; set; } = new List<Bus>();

    }
}
