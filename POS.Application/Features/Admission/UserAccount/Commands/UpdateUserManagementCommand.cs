using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Admission.UserAccount.Commands
{
    public record UpdateUserManagementCommand(
        long Id,
        string FullName,
        string Username,
        string Email,
        string MobileNumber,
        long RoleId,
        bool IsActive) : IRequest<ApiResponses<bool>>;

    public class UpdateUserManagementCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<UpdateUserManagementCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateUserManagementCommand request, CancellationToken cancellationToken)
        {
            var usersRepo = unitOfWork.GetRepository<Users>();

            var user = await usersRepo.GetFirstOrDefault<Users>(
                selector: null,
                predicate: u => u.Id == request.Id,
                orderBy: null,
                include: null,
                disableTracking: false);

            if (user == null)
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "User not found.");

            var duplicateExists = await usersRepo.AnyAsync(u => u.Id != request.Id &&
                (u.UserName == request.Username || u.Email == request.Email));

            if (duplicateExists)
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "Username or Email address is already registered to another user.");

            var (firstName, lastName) = SplitName(request.FullName);

            user.FullName = request.FullName.Trim();
            user.FirstName = firstName;
            user.LastName = lastName;
            user.UserName = request.Username.Trim();
            user.Email = request.Email.Trim();
            user.Mobile = request.MobileNumber.Trim();
            user.RoleId = request.RoleId;
            user.IsActive = request.IsActive;
            user.ModifiedDate = DateTime.UtcNow;

            await usersRepo.Update(user);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "User updated successfully.");
        }

        private static (string FirstName, string LastName) SplitName(string? fullName)
        {
            var names = (fullName ?? string.Empty).Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = names.Length > 0 ? names[0] : string.Empty;
            var lastName = names.Length > 1 ? string.Join(' ', names.Skip(1)) : firstName;
            return (firstName, lastName);
        }
    }
}