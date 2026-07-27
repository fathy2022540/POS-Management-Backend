namespace JRM.Application.Common.DTOs
{
    public class JWTConfiguration
    {
        public string? ValidAudience { get; set; }
        public string? ValidIssuer { get; set; }
        public string? SecretKey { get; set; }
        public int AccessExpiration { get; set; }
        public int JWT_RefreshTokenExpiration { get; set; }
    }
}
