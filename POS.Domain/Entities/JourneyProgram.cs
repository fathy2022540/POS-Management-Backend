using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class JourneyProgram : BaseEntity
    {
        public long JourneyId { get; set; }

        public int DayNumber { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public required string Title { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public required string Description { get; set; }

        // Optional details for specific scheduling
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Location { get; set; }
        public Journey? Journey { get; set; }

    }
}
