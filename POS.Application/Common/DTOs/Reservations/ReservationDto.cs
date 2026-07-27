

using JRM.Domain.Enums;

namespace JRM.Application.Common.DTOs
{
    public class CreateReservationDto
    {
        public long JourneyId { get; set; }
        public string ClientName { get; set; } = null!;
        public string ClientEmail { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public int NumberOfSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public List<long>? SeatIds { get; set; }
        public List<HotelReservationDto>? HotelReservations { get; set; }
    }

    public class HotelReservationDto
    {
        public long HotelId { get; set; }
        public RoomType RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal RoomPriceTotal { get; set; }
    }

    public class UpdateReservationDto
    {
        public long Id { get; set; }
        public string ClientName { get; set; } = null!;
        public string ClientEmail { get; set; } = null!;
        public int NumberOfSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
