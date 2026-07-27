using JRM.Application.Common.DTOs;

namespace JRM.Application.Helper
{
    public class SystemSettingsConfiguration : IDisposable
    {
        public AppSettingsConfiguration AppSettingsConfiguration { get; set; }
        public JWTConfiguration JWTConfiguration { get; set; }
        public string[] CorsUrls { get; set; }

        public void Dispose()
        {
        }
    }
    public record AppSettingsConfiguration(string ConnectionStrings, bool enableSwagger, string AttachementStorage);
}
