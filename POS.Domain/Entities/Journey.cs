using JRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class Journey : BaseEntity
    {
        [Column(TypeName = "nvarchar(200)")]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public JourneyType Type { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public long? CompanyId { get; set; }
        public bool RequiresHotel { get; set; } = false;
        public long? BusId { get; set; }
        public long? HotelId { get; set; }
        public Company? Company { get; set; }
        public Bus? Bus { get; set; }
        public Hotels? Hotel { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<JourneyProgram> Programs { get; set; } = new List<JourneyProgram>();

    }
}
