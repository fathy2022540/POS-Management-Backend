using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Admission.PermissionServices.Queries
{
    public record GetPermissionMatrixQuery(long RoleId, long? UserId) : IRequest<ApiResponses<IEnumerable<PermissionMatrixRowDto>>>;

    public class GetPermissionMatrixQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetPermissionMatrixQuery, ApiResponses<IEnumerable<PermissionMatrixRowDto>>>
    {
        public async Task<ApiResponses<IEnumerable<PermissionMatrixRowDto>>> Handle(GetPermissionMatrixQuery request, CancellationToken cancellationToken)
        {
            var screensRepo = unitOfWork.GetRepository<SystemScreens>();
            var rolePermsRepo = unitOfWork.GetRepository<RoleScreenPermissions>();
            var userOverridesRepo = unitOfWork.GetRepository<UserScreenPermissionOverrides>();

            var screens = await screensRepo.GetByCriteriaQueryable(s => true)
                .AsNoTracking()
                .Include(s => s.Module)
                .OrderBy(s => s.Module!.SortOrder)
                .ThenBy(s => s.SortOrder)
                .ToListAsync(cancellationToken);

            var rolePerms = await rolePermsRepo.GetByCriteriaQueryable(x => x.RoleId == request.RoleId)
                .AsNoTracking()
                .ToDictionaryAsync(x => x.ScreenId, x => x, cancellationToken);

            var userOverrides = new Dictionary<long, UserScreenPermissionOverrides>();
            if (request.UserId.HasValue)
            {
                userOverrides = await userOverridesRepo.GetByCriteriaQueryable(x => x.UserId == request.UserId.Value)
                    .AsNoTracking()
                    .ToDictionaryAsync(x => x.ScreenId, x => x, cancellationToken);
            }

            var rows = screens.Select(s =>
            {
                rolePerms.TryGetValue(s.Id, out var basePerm);
                userOverrides.TryGetValue(s.Id, out var ov);

                return new PermissionMatrixRowDto
                {
                    ScreenId = s.Id,
                    ScreenCode = s.ScreenCode,
                    ScreenNameEn = s.ScreenName,
                    ScreenNameAr = s.ScreenNameAR,
                    Module = s.Module != null ? s.Module.Name : string.Empty,
                    View = ov?.CanView ?? basePerm?.CanView ?? false,
                    Create = ov?.CanCreate ?? basePerm?.CanCreate ?? false,
                    Edit = ov?.CanEdit ?? basePerm?.CanEdit ?? false,
                    Delete = ov?.CanDelete ?? basePerm?.CanDelete ?? false,
                    Approve = ov?.CanApprove ?? basePerm?.CanApprove ?? false,
                    Export = ov?.CanExport ?? basePerm?.CanExport ?? false
                };
            }).ToList();

            return ApiResponses<IEnumerable<PermissionMatrixRowDto>>.Success(rows, "Permission matrix retrieved successfully.");
        }
    }
}