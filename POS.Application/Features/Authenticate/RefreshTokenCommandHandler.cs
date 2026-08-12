using MediatR;
using Microsoft.IdentityModel.Tokens;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper.Authentication;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Authenticate
{
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<ApiResponses<RefreshTokenModel>>;

    public class RefreshTokenCommandHandler(ITokenService tokenService, IUnitOfWork<POSDBContext> unitOfWork)
            : IRequestHandler<RefreshTokenCommand, ApiResponses<RefreshTokenModel>>
    {
        public async Task<ApiResponses<RefreshTokenModel>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Structural verification of security signatures
                var principal = tokenService.GetPrincipalFromExpiredToken(request.AccessToken);

                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == Claims.UserId)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
                {
                    return ApiResponses<RefreshTokenModel>.Failure(StatusResult.ModelNotValid, "Invalid access token.");
                }

                // 2. Validate token dates
                var refreshExpiry = tokenService.GetExpiredDateRefreshToken(request.RefreshToken);
                if (refreshExpiry == null || refreshExpiry < DateTime.UtcNow)
                {
                    return ApiResponses<RefreshTokenModel>.Failure(StatusResult.ModelNotValid, "The refresh token has expired.");
                }

                // Fetch user from the database directly
                var usersRepo = unitOfWork.GetRepository<Users>();
                var user = await usersRepo.GetFirstOrDefault<Users>(null, u => u.Id == userId, null, null, false);

                if (user == null)
                {
                    return ApiResponses<RefreshTokenModel>.Failure(StatusResult.NotFound, "User not found.");
                }

                var mappedUser = AppMapper.Mapper.Map<UserModel>(user);

                var rememberMeClaim = principal.Claims.FirstOrDefault(c => c.Type == Claims.RemeberMe)?.Value;
                bool.TryParse(rememberMeClaim, out bool rememberMe);

                // 3. Request fresh generation securely
                string newRefreshToken = tokenService.GenerateRefreshToken(mappedUser);
                string newAccessToken = tokenService.GenerateAccessToken(mappedUser, newRefreshToken, rememberMe);

                var response = new RefreshTokenModel { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
                return ApiResponses<RefreshTokenModel>.Success(response, "Token refreshed successfully.");
            }
            catch (SecurityTokenException ex)
            {
                return ApiResponses<RefreshTokenModel>.Failure(StatusResult.ModelNotValid, ex.Message);
            }
        }
    }
}