using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.PermissionServices.Commands
{
    public record DeletePermissionCommand(long Id) : IRequest<ApiResponses<bool>>;

    public class DeletePermissionCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<DeletePermissionCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(DeletePermissionCommand request, CancellationToken cancellationToken)
        {
            var permissionRepo = unitOfWork.GetRepository<Permissions>();

            // 1. Verify existence
            var existingPermission = await permissionRepo.Find(request.Id);
            if (existingPermission == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Permission not found.");
            }

            // 2. Fetch linked relations
            var rolePermissionsRepo = unitOfWork.GetRepository<RolesPermissions>();
            var linkedRelations = await rolePermissionsRepo.GetList(
                rp => rp.PermissionId == request.Id,
                null, null,
                disableTracking: false
            );

            var relationsList = linkedRelations.ToList();

            // 3. Delete dependent relations first (Active code)
            if (relationsList.Any())
            {
                foreach (var relation in relationsList)
                {
                    await rolePermissionsRepo.Delete(relation);
                }
            }

            // 4. Delete the master permission
            await permissionRepo.Delete(existingPermission);

            // 5. Save everything
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Permission deleted successfully.");
        }
    }
}