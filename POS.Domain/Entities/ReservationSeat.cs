using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class ReservationSeat : BaseEntity
    {
        public long ReservationId { get; set; }
        public long SeatId { get; set; }
        public Seat? Seat { get; set; }
        public Reservation? Reservation { get; set; }

    }
}
