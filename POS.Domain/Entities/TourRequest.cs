using JRM.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JRM.Domain.Entities
{
    public class TourRequest : BaseEntity
    {
        [Column(TypeName = "nvarchar(200)")]
        public required string ClientName { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public required string ClientEmail { get; set; }
        
        [Column(TypeName = "nvarchar(300)")]
        public required string Destination { get; set; }
        public JourneyType PreferredType { get; set; }

        public DateTime? PreferredStartDate { get; set; }
        public DateTime? PreferredEndDate { get; set; }

        public string? SpecificRequirements { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

    }
}
