using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class Bus : BaseEntity
    {
        [Column(TypeName = "nvarchar(150)")]
        public required string PlateNumber { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public required string BusModel { get; set; }
        public int TotalCapacity { get; set; }

        // Foreign Key to the Tour Company
        public long CompanyId { get; set; }
        public Company? Company { get; set; }

        // Navigation properties
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public ICollection<Journey> Journeys { get; set; } = new List<Journey>();

    }
}
