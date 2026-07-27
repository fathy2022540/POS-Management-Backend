using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.RolesServices.Commands.Delete
{
    public record DeleteRoleCommand(long id) : IRequest<ApiResponses<bool>>;
    public class DeleteRolesCommandHandler(
        IUnitOfWork<JRMDBContext> unitOfWork,
        ILogger<DeleteRolesCommandHandler> logger)
        : IRequestHandler<DeleteRoleCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Attempting to delete role with ID: {RoleId}", request.id);

                // 1. Validate: Role exists
                var rolesRepo = unitOfWork.GetRepository<Roles>();
                var existingRole = await rolesRepo.Find(request.id);

                if (existingRole == null)
                {
                    //logger.LogWarning("Role with ID {RoleId} not found.", request.RoleId);
                    return ApiResponses<bool>.Failure(
                        StatusResult.NotFound,
                        $"Role with ID {request.id} does not exist.");
                }

                // 2. Data Integrity Check: Prevent deletion if role is assigned to users
                var userRolesRepo = unitOfWork.GetRepository<RoleScreenPermissions>();
                
                var assignedUsersCount = await userRolesRepo
                    .GetByCriteriaQueryable(ur => ur.RoleId == request.id)
                    .CountAsync(cancellationToken);

                if (assignedUsersCount > 0)
                {
                    //logger.LogWarning(
                    //    "Cannot delete role {RoleId} ({RoleName}) because it is assigned to {UserCount} user(s).",
                    //    request.RoleId,
                    //    existingRole.NameEn,
                    //    assignedUsersCount);

                    return ApiResponses<bool>.Failure(
                        StatusResult.NotAllowedDelete,
                        $"Cannot delete this role. It is currently assigned to {assignedUsersCount} user(s). " +
                        "Please reassign users to other roles first.");
                }

                // 3. Remove related permissions (RolePermissions) if Cascade Delete is not configured
                var rolePermissionsRepo = unitOfWork.GetRepository<RolesPermissions>();
                var relatedPermissions = await rolePermissionsRepo
                    .GetByCriteriaQueryable(rp => rp.RoleId == request.id)
                    .ToListAsync(cancellationToken);

                if (relatedPermissions.Count > 0)
                {
                    //logger.LogInformation(
                    //    "Removing {PermissionCount} role-permission associations for role {RoleId}.",
                    //    relatedPermissions.Count,
                    //    request.RoleId);

                    foreach (var rolePermission in relatedPermissions)
                    {
                        await rolePermissionsRepo.Delete(rolePermission);
                    }
                }

                // 4. Delete the role
                await rolesRepo.Delete(existingRole);
                await unitOfWork.DoWork();

            

                return ApiResponses<bool>.Success(
                    true,
                    $"Role '{existingRole.NameEn}' has been deleted successfully.");
            }
            catch (DbUpdateException ex)
            {


                return ApiResponses<bool>.Failure(
                    StatusResult.InternalServerError,
                    "A database error occurred while deleting the role. Please try again later.");
            }
            catch (Exception ex)
            {
         

                return ApiResponses<bool>.Failure(
                    StatusResult.InternalServerError,
                    "An unexpected error occurred while deleting the role.");
            }
        }
    }
}