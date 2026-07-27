namespace JRM.Application.Common.DTOs
{
    public class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
        public string userName { get; set; }
        public string fullName { get; set; }
        public long? userId { get; set; }
        public long? RoleId { get; set; }
    }
    public class RefreshToken
    {
        public string TokenString { get; set; }
        public DateTime ExpireAt { get; set; }
    }
    public class TokenSetting
    {
        public string access_token { get; set; } = "";
        public int expires_in { get; set; } = 0;
        public string token_type { get; set; } = "";
        public string scope { get; set; } = "";
    }
}
