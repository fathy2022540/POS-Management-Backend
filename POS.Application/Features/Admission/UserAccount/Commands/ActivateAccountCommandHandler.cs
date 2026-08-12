using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.UserAccount.Commands
{
    public record ActivateAccountCommand(string Token, string Email) : IRequest<ApiResponses<bool>>;

    // 2. The Handler
    public class ActivateAccountCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<ActivateAccountCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
        {
            var userRepo = unitOfWork.GetRepository<Users>();
            // Fetch the user using an extension method or query matching the token/email
            var user = await userRepo.GetFirstOrDefault<Users>(null, u => u.Email == request.Email && u.ActivationToken == request.Token, null, null, true);

            if (user == null)
                return ApiResponses<bool>.Failure(StatusResult.NotFoundInActiveDirectory, " Email address or ActivationToken is invalid.");

            user.IsActive = true;
            user.ActivationToken = null; // Wipe out token once consumed safely

            await userRepo.Update(user);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "User is activated successfully");
        }
    }
}