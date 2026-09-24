using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper.Authentication;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Authenticate
{
    public record LoginCommand(string? Username, string? Password, bool RememberMe = false)
        : IRequest<ApiResponses<LoginResponse>>;

    public record LoginResponse(long UserId, string UserName, string FullName, string AccessToken, string RefreshToken);

    public class LoginCommandHandler(
        IUnitOfWork<POSDBContext> unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
        : IRequestHandler<LoginCommand, ApiResponses<LoginResponse>>
    {
        public async Task<ApiResponses<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate Input
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return ApiResponses<LoginResponse>.Failure(
                    StatusResult.InvalidPassword,
                    "Username and password are required.");
            }

            var usersRepo = unitOfWork.GetRepository<Users>();

            // 2. Fetch User with Cancellation Token
            var user = await usersRepo.GetFirstOrDefault<Users>(
                predicate: c => c.UserName == request.Username,
                disableTracking: true);

            if (user == null)
            {
                return ApiResponses<LoginResponse>.Failure(
                    StatusResult.NotFoundUserAndPasswordAuthentication,
                    "User not found.");
            }

            // 3. Check User Active Status 
            if (user.IsActive == false)
            {
                return ApiResponses<LoginResponse>.Failure(
                    StatusResult.LockedOut,
                    "User account is inactive. Please contact system administrator.");
            }

            // 4. Validate Password
            if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return ApiResponses<LoginResponse>.Failure(
                    StatusResult.InvalidPassword,
                    "Invalid username or password.");
            }

            var mappedUser = AppMapper.Mapper.Map<UserModel>(user);

            // 5. Generate fresh JWT pair
            string refreshToken = tokenService.GenerateRefreshToken(mappedUser);
            string accessToken = tokenService.GenerateAccessToken(mappedUser, refreshToken, rememberMe: request.RememberMe);

            // 6. Save/Update Refresh Token in DB
            //user.RefreshToken = refreshToken;
            //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            //await unitOfWork.SaveChangesAsync();

            // 7. Return Success Response
            var loginData = new LoginResponse(user.Id, user.UserName, user.FullName, accessToken, refreshToken);

            return ApiResponses<LoginResponse>.Success(loginData, "Logged in successfully.");
        }
    }
}