

using JRM.Domain.Enums;

namespace JRM.Application.Common.DTOs
{
    public class CreateJourneyDto
    {
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
    }
    public class UpdateJourneyDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public bool RequiresHotel { get; set; }
    }
}
