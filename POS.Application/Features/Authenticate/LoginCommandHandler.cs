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

    public record LoginCommand(string? username, string? password, bool RememberMe = false)
        : IRequest<ApiResponses<LoginResponse>>;

    public record LoginResponse(long UserId, string UserName, string FullName, string AccessToken, string RefreshToken);

    public class LoginCommandHandler(IUnitOfWork<POSDBContext> unitOfWork, IPasswordHasher passwordHasher, ITokenService tokenService)
        : IRequestHandler<LoginCommand, ApiResponses<LoginResponse>>
    {
        public async Task<ApiResponses<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var usersRepo = unitOfWork.GetRepository<Users>();

            var user = await usersRepo.GetFirstOrDefault<Users>(null, c => c.UserName == request.username, null, null, false);
            if (user == null)
            {
                return ApiResponses<LoginResponse>.Failure(StatusResult.NotFoundUserAndPasswordAuthentication, "User Not Found");
            }

            var mappedUser = AppMapper.Mapper.Map<UserModel>(user);

            // 2. Validate Password using Verification
            if (!passwordHasher.VerifyPassword(request.password, user.PasswordHash))
            {
                var res = ApiResponses<LoginResponse>.Failure(StatusResult.InvalidPassword, "Invalid username or password.");
                return res;
            }

            // 3. Generate fresh JWT pair
            string refreshToken = tokenService.GenerateRefreshToken(mappedUser);
            string accessToken = tokenService.GenerateAccessToken(mappedUser, refreshToken, rememberMe: request.RememberMe);

            // 4. Wrap the response inside ApiResponses<>.Success
            var loginData = new LoginResponse(user.Id, user.UserName, user.FullName, accessToken, refreshToken);
            var resu = ApiResponses<LoginResponse>.Success(loginData, "Logged in successfully.");
            return resu;
        }
    }
}