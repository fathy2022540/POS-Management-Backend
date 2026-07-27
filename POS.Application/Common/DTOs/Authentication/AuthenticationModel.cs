namespace JRM.Application.Common.DTOs
{
    public record AuthenticationModel(string? username, string? password, bool IsRememberMe);
}
