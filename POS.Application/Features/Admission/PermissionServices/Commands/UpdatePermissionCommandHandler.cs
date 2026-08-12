using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.PermissionServices.Commands
{
    public record UpdatePermissionCommand(long Id, string Code, string Name) : IRequest<ApiResponses<bool>>;

    public class UpdatePermissionCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<UpdatePermissionCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            var permissionRepo = unitOfWork.GetRepository<Permissions>();

            var existingPermission = await permissionRepo.Find(request.Id);
            if (existingPermission == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Permission not found.");
            }

            AppMapper.Mapper.Map(request, existingPermission);

            await permissionRepo.Update(existingPermission);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Permission updated successfully.");
        }
    }
}
