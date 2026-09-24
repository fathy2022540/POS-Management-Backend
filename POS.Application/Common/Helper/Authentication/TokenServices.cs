using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using POS.Application.Common.DTOs;
using POS.Application.Helper;

namespace POS.Application.Common.Helper.Authentication
{
    public interface ITokenService
    {
        (string Token, string RefreshToken) GenerateTokens(UserModel user, bool rememberMe, string? existingRefreshToken);
        string GenerateAccessToken(UserModel user, string? existingRefreshToken, bool rememberMe);
        string GenerateRefreshToken(UserModel user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        DateTime? GetExpiredDateRefreshToken(string refreshToken);
    }

    public class TokenService(SystemSettingsConfiguration systemSettings) : ITokenService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly JWTConfiguration _jwtConfig = systemSettings.JWTConfiguration;

        private SecurityKey GetSigningKey()
        {
            string secret = _jwtConfig.SecretKey ?? "POS_System_Default_Super_Secret_Key_2026_Long_Enough";
            byte[] keyBytes = Encoding.UTF8.GetBytes(secret);
            return new SymmetricSecurityKey(keyBytes);
        }

        private TokenValidationParameters GetValidationParameters() => new()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetSigningKey(),
            ValidateLifetime = false
        };

        public (string Token, string RefreshToken) GenerateTokens(UserModel user, bool rememberMe, string? existingRefreshToken)
        {
            var tokenHandler = new JsonWebTokenHandler();
            var signingCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256Signature);

            var utcNow = DateTime.UtcNow;
            var expiresAt = utcNow.AddMinutes(_jwtConfig.AccessExpiration);
            string userDisplayName = user.UserName?.TrimEnd('.') ?? string.Empty;

            // 1. Generate Access Token
            var claims = new Dictionary<string, object>
            {
                { ClaimTypes.Name, userDisplayName },
                { ClaimTypes.Email, user.Email ?? string.Empty },
                { Claims.UserId, user.Id.ToString() },
                { Claims.UserData, JsonSerializer.Serialize(UserClaimModel.CreateUserClaimModel(user)) },
                { Claims.RemeberMe, rememberMe.ToString().ToLower() },
                { Claims.ExpiresAt, expiresAt.ToString("g", CultureInfo.InvariantCulture) }
            };

            if (!string.IsNullOrEmpty(existingRefreshToken))
            {
                claims.Add(Claims.RefreshToken, existingRefreshToken);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Claims = claims,
                IssuedAt = utcNow,
                Expires = expiresAt,
                Issuer = _jwtConfig.ValidIssuer,
                Audience = _jwtConfig.ValidAudience,
                SigningCredentials = signingCredentials
            };

            string accessToken = tokenHandler.CreateToken(tokenDescriptor);

            // 2. Generate Refresh Token
            var refreshTokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object> { { "userid", user.Id.ToString() } },
                IssuedAt = utcNow,
                Expires = utcNow.AddDays(_jwtConfig.JWT_RefreshTokenExpiration),
                SigningCredentials = signingCredentials
            };

            string generatedRefreshToken = tokenHandler.CreateToken(refreshTokenDescriptor);

            return (accessToken, generatedRefreshToken);
        }

        public string GenerateAccessToken(UserModel user, string? existingRefreshToken, bool rememberMe)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(systemSettings.JWTConfiguration.AccessExpiration);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName?.TrimEnd('.') ?? string.Empty, ClaimValueTypes.String),
                new(ClaimTypes.Email, user.Email ?? string.Empty, ClaimValueTypes.String),
                new(Claims.UserId, user.Id.ToString()),
                new(Claims.UserData, JsonSerializer.Serialize(UserClaimModel.CreateUserClaimModel(user))),
                new(Claims.RemeberMe, rememberMe.ToString().ToLower()),
                new(Claims.ExpiresAt, expiresAt.ToString(new CultureInfo("en-GB")))
            };

            if (!string.IsNullOrEmpty(existingRefreshToken))
            {
                claims.Add(new(Claims.RefreshToken, existingRefreshToken));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = expiresAt,
                Issuer = systemSettings.JWTConfiguration.ValidIssuer,
                Audience = systemSettings.JWTConfiguration.ValidAudience,
                SigningCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            return _tokenHandler.WriteToken(_tokenHandler.CreateToken(tokenDescriptor));
        }

        public string GenerateRefreshToken(UserModel user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("userid", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(systemSettings.JWTConfiguration.JWT_RefreshTokenExpiration),
                SigningCredentials = new SigningCredentials(GetSigningKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            return _tokenHandler.WriteToken(_tokenHandler.CreateToken(tokenDescriptor));
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var principal = _tokenHandler.ValidateToken(token, GetValidationParameters(), out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token signature");
            }

            return principal;
        }

        public DateTime? GetExpiredDateRefreshToken(string refreshToken)
        {
            var principal = _tokenHandler.ValidateToken(refreshToken, GetValidationParameters(), out _);
            var exp = principal.Claims.FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Exp)?.Value;

            return exp != null ? DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime : null;
        }
    }
}