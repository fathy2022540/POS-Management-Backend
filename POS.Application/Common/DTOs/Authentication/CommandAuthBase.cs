namespace JRM.Application.Common.DTOs
{
    public class CommandAuthBase
    {
        public string? AccessToken { get; set; }
        public UserModel? User { get; set; }
        public string? IpAddress { get; set; }
    }
}
