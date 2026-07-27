using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Common.Helper;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.PermissionServices.Commands
{
    public record CreatePermissionCommand(string Code, string Name) : IRequest<ApiResponses<bool>>;

    public class CreatePermissionCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<CreatePermissionCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            var permissionRepo = unitOfWork.GetRepository<Permissions>();

            // Check if Code already exists (Codes should be unique identifiers like "USER_CREATE")
            var codeExists = await BusinessValidator.FindConflictAsync(permissionRepo, v => v.Code.ToLower() == request.Code.ToLower());
            if (codeExists != null)
            {
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "Permission code already exists.");
            }

            var newPermission = AppMapper.Mapper.Map<Permissions>(request);

            await permissionRepo.Insert(newPermission);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Permission created successfully.");
        }
    }
}