namespace POS.Application.Common.DTOs
{
    public record AuthenticationModel(string? username, string? password, bool IsRememberMe);
    public record AuthentErrorStatusModel(string? errorCode, string? statuMsg, string? statusCode);
    public class CommandAuthBase
    {
        public string? AccessToken { get; set; }
        public UserModel? User { get; set; }
        public string? IpAddress { get; set; }
    }
}
