using JRM.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class Reservation : BaseEntity
    {
        public long JourneyId { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public required string ClientName { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public required string ClientEmail { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public int NumberOfSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public Journey? Journey { get; set; }
        public ICollection<ReservationSeat> ReservedSeats { get; set; } = new List<ReservationSeat>();
        public ICollection<HotelReservation> HotelReservations { get; set; } = new List<HotelReservation>();

    }
}
