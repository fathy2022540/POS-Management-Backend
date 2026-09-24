using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Admission.UserAccount.Commands
{
    public record DeleteUserManagementCommand(long Id) : IRequest<ApiResponses<bool>>;

    public class DeleteUserManagementCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<DeleteUserManagementCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(DeleteUserManagementCommand request, CancellationToken cancellationToken)
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

            await usersRepo.Delete(user);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "User deleted successfully.");
        }
    }
}