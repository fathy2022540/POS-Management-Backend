namespace JRM.Application.Common.DTOs
{
    public class NotificationItemResponse
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string MessageEn { get; set; } = string.Empty;
        public string MessageAr { get; set; } = string.Empty;
        public string? ReferenceType { get; set; }
        public long? ReferenceId { get; set; }
        public string? ActionUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ReadAt { get; set; }
    }
}