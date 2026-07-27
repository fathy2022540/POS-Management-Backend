namespace JRM.Application.Common.DTOs
{
    public class UserNotificationsResponse
    {
        public int UnreadCount { get; set; }
        public IEnumerable<NotificationItemResponse> Items { get; set; } = Enumerable.Empty<NotificationItemResponse>();
    }
}