using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.RolesServices.Commands
{
    public record UpdateRoleStatusCommand(long Id, bool IsActive) : IRequest<ApiResponses<bool>>;
    public class UpdateRoleStatusCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<UpdateRoleStatusCommand, ApiResponses<bool>>
    {
    

        public async Task<ApiResponses<bool>> Handle(UpdateRoleStatusCommand request, CancellationToken cancellationToken)
        {
            var roleRepo = unitOfWork.GetRepository<Roles>();

            var role = await roleRepo.GetFirstOrDefault<Roles>(null, r => r.Id == request.Id, null, null, true);
            if (role == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "الدور غير موجود.");
            }

            role.IsActive = request.IsActive;
            await roleRepo.Update(role);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "تم تحديث حالة الدور بنجاح.");
        }
    }
}
