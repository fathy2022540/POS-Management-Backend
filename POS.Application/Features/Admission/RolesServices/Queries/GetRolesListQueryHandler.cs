using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.RolesServices.Queries
{
    public record GetRolesListQuery : IRequest<ApiResponses<IEnumerable<object>>>;

    public class GetRolesListQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetRolesListQuery, ApiResponses<IEnumerable<object>>>
    {
        public async Task<ApiResponses<IEnumerable<object>>> Handle(GetRolesListQuery request, CancellationToken cancellationToken)
        {
            var rolesRepo = unitOfWork.GetRepository<Domain.Entities.Roles>();
            var usersRepo = unitOfWork.GetRepository<Domain.Entities.Users>();

            var roles = await rolesRepo.GetByCriteriaQueryable(r => true)
                .AsNoTracking()
                .Select(r => new
                {
                    id = r.Id,
                    code = r.Code,
                    name = r.NameEn,
                    description = r.NameAr,
                    active = r.IsActive,
                    usersCount = usersRepo.GetByCriteriaQueryable(u => u.RoleId == r.Id).Count()
                })
                .OrderBy(r => r.name)
                .ToListAsync(cancellationToken);

            return ApiResponses<IEnumerable<object>>.Success(roles, "Roles retrieved successfully.");
        }
    }
}