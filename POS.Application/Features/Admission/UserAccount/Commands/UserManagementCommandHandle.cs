using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.UserAccount.Commands
{
    public record UserManagementCommand(
        string FullName,
        string Username,
        string Email,
        string MobileNumber,
        string Password,
        long RoleId,
        long? StatusId,
        long? UserTypeId,
        bool IsActive) : IRequest<ApiResponses<bool>>;

    public class UserManagementCommandHandle(IUnitOfWork<POSDBContext> unitOfWork,
      IPasswordHasher passwordHasher) : IRequestHandler<UserManagementCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UserManagementCommand request, CancellationToken cancellationToken)
        {
            var normalizedRequest = NormalizeRequest(request);

            var UsersRepo = unitOfWork.GetRepository<Users>();

            // Send all conditions into a single predicate expression using OR (||)
            var userExists = await BusinessValidator.FindConflictAsync(UsersRepo,
                u => u.UserName == normalizedRequest.Username || u.Email == normalizedRequest.Email
            );

            if (userExists != null)
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "Username or Email address is already registered.");

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {

                var newUser = AppMapper.Mapper.Map<Users>(normalizedRequest, opt =>
                {
                    opt.Items["PasswordHasher"] = passwordHasher;
                });

                await UsersRepo.Insert(newUser);
                await unitOfWork.DoWork();
                // 6. Trigger external notification dispatcher pipelines
                // await emailService.SendActivationEmailAsync(newUser.Email, newUser.UserName, activationToken);

                // Commit atomic modifications securely to disk
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return ApiResponses<bool>.Success(true, "User created successfully");
            }
            catch (Exception ex)
            {
                // Something went down! Revert modifications cleanly from the server environment
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                return ApiResponses<bool>.Failure(StatusResult.InvalidRequest, $"Registration workflow crashed: {ex.Message}");

            }
        }

        private static UserManagementCommand NormalizeRequest(UserManagementCommand request)
        {
            return request with
            {
                FullName = request.FullName?.Trim() ?? string.Empty,
                Username = request.Username?.Trim() ?? string.Empty,
                Email = request.Email?.Trim() ?? string.Empty,
                MobileNumber = request.MobileNumber?.Trim() ?? string.Empty,
                IsActive = request.IsActive,
                RoleId = request.RoleId,
            };
        }
    }
}