using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class Seat : BaseEntity
    {
        [Column(TypeName = "nvarchar(50)")]
        public required string SeatNumber { get; set; }

        // Foreign Key to the specific Bus
        public long BusId { get; set; }
        public Bus? Bus { get; set; }

        // Navigation property tracking reservations over time
        public ICollection<ReservationSeat> ReservationSeats { get; set; } = new List<ReservationSeat>();

    }
}
