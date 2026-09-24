using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Admission.PermissionServices.Queries
{
    public record SavePermissionMatrixCommand(SavePermissionMatrixRequest Request) : IRequest<ApiResponses<bool>>;

    public class SavePermissionMatrixCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<SavePermissionMatrixCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(SavePermissionMatrixCommand requestCommand, CancellationToken cancellationToken)
        {
            var request = requestCommand.Request;
            var rolePermsRepo = unitOfWork.GetRepository<RoleScreenPermissions>();
            var userOverridesRepo = unitOfWork.GetRepository<UserScreenPermissionOverrides>();

            if (request.UserId.HasValue)
            {
                var rolePerms = await rolePermsRepo.GetByCriteriaQueryable(x => x.RoleId == request.RoleId)
                    .AsNoTracking()
                    .ToDictionaryAsync(x => x.ScreenId, x => x, cancellationToken);

                var existingOverrides = await userOverridesRepo.GetByCriteriaQueryable(x => x.UserId == request.UserId.Value)
                    .ToListAsync(cancellationToken);

                foreach (var item in existingOverrides)
                {
                    await userOverridesRepo.Delete(item);
                }

                var overrides = request.Rows
                    .Where(r =>
                    {
                        rolePerms.TryGetValue(r.ScreenId, out var rolePerm);
                        return (rolePerm?.CanView ?? false) != r.View ||
                               (rolePerm?.CanCreate ?? false) != r.Create ||
                               (rolePerm?.CanEdit ?? false) != r.Edit ||
                               (rolePerm?.CanDelete ?? false) != r.Delete ||
                               (rolePerm?.CanApprove ?? false) != r.Approve ||
                               (rolePerm?.CanExport ?? false) != r.Export;
                    })
                    .Select(r => new UserScreenPermissionOverrides
                    {
                        UserId = request.UserId.Value,
                        ScreenId = r.ScreenId,
                        CanView = r.View,
                        CanCreate = r.Create,
                        CanEdit = r.Edit,
                        CanDelete = r.Delete,
                        CanApprove = r.Approve,
                        CanExport = r.Export,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = 1
                    }).ToList();

                foreach (var ov in overrides)
                {
                    await userOverridesRepo.Insert(ov);
                }
            }
            else
            {
                var rolePerms = await rolePermsRepo.GetByCriteriaQueryable(x => x.RoleId == request.RoleId)
                    .ToListAsync(cancellationToken);

                foreach (var item in rolePerms)
                {
                    await rolePermsRepo.Delete(item);
                }

                var newRolePerms = request.Rows.Select(r => new RoleScreenPermissions
                {
                    RoleId = request.RoleId,
                    ScreenId = r.ScreenId,
                    CanView = r.View,
                    CanCreate = r.Create,
                    CanEdit = r.Edit,
                    CanDelete = r.Delete,
                    CanApprove = r.Approve,
                    CanExport = r.Export,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = 1
                }).ToList();

                foreach (var rp in newRolePerms)
                {
                    await rolePermsRepo.Insert(rp);
                }
            }

            await unitOfWork.DoWork();
            return ApiResponses<bool>.Success(true, "Permission matrix saved successfully.");
        }
    }
}