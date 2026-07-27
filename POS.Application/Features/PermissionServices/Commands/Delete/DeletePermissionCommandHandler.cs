using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.PermissionServices.Commands
{


    public record DeletePermissionCommand(long Id) : IRequest<ApiResponses<bool>>;

    public class DeletePermissionCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
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

            // 2. Fetch linked relations (Make sure tracking is ENABLED for deletions)
            var rolePermissionsRepo = unitOfWork.GetRepository<RolesPermissions>();

            // Explicitly fetch as an active tracking list from the database
            var linkedRelations = await rolePermissionsRepo.GetList(
                rp => rp.PermissionId == request.Id,
                null, null,
                disableTracking: false // MUST BE FALSE so EF can track and remove them!
            );

            var relationsList = linkedRelations.ToList();

            // 3. Delete dependent relations first
            if (relationsList.Any())
            {
                // IF YOUR REPOSITORY HAS DeleteRange:
                // await rolePermissionsRepo.DeleteRange(relationsList);

                // IF YOUR REPOSITORY ONLY HAS Delete:
                // Crucial Fix: In 99% of generic repositories, .Delete() marks the state as Deleted. 
                // It modifies local state, so do NOT await it inside a loop unless it hits the DB immediately.
                foreach (var relation in relationsList)
                {
                    // If your repo method returns a Task, await it. If it's void, remove 'await'
                    //  await rolePermissionsRepo.Delete(relation);
                }
            }

            // 4. Delete the master permission
            await permissionRepo.Delete(existingPermission);

            // 5. Save everything to the DB in a single atomic transaction
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Permission deleted successfully.");
        }
    }
}
