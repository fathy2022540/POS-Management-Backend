using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.PermissionServices.Commands
{
    public record UpdatePermissionCommand(long Id, string Code, string Name) : IRequest<ApiResponses<bool>>;

    public class UpdatePermissionCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
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
