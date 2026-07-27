using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Common.Helper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.PermissionServices.Commands
{
    public record AssignPermissionToRoleCommand(long RoleId, long PermissionId) : IRequest<ApiResponses<bool>>;

    public class AssignPermissionToRoleCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<AssignPermissionToRoleCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(AssignPermissionToRoleCommand request, CancellationToken cancellationToken)
        {
            var rolePermissionsRepo = unitOfWork.GetRepository<RolesPermissions>();

            // 1. Check if relation already exists
            var alreadyAssigned = await BusinessValidator.FindConflictAsync(rolePermissionsRepo, rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId);
            if (alreadyAssigned != null)
            {
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "This permission is already assigned to this role.");
            }

            // 2. Validate that both the Role and Permission actually exist
            var roleRepo = unitOfWork.GetRepository<Roles>();
            var permissionRepo = unitOfWork.GetRepository<Permissions>();
            var roleExists = await BusinessValidator.FindConflictAsync(roleRepo, r => r.Id == request.RoleId);
            var permissionExists = await BusinessValidator.FindConflictAsync(permissionRepo, p => p.Id == request.PermissionId);
            if ((roleExists != null) || (permissionExists != null))
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Role or Permission does not exist.");
            }

            // 3. Create mapping relation (Assuming properties on RolesPermissions match standard naming)
            var rolePermission = new RolesPermissions
            {
                RoleId = request.RoleId,
                PermissionId = request.PermissionId
            };

            await rolePermissionsRepo.Insert(rolePermission);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Permission assigned to role successfully.");
        }
    }
}
