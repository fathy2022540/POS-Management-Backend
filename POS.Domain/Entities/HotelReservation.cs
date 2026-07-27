using JRM.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class HotelReservation : BaseEntity
    {
        public long HotelId { get; set; }
        public long ReservationId { get; set; }
        public RoomType RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal RoomPriceTotal { get; set; }
        public Reservation? Reservation { get; set; }
        public Hotels? Hotel { get; set; }

    }
}
