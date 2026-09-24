using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.RolesServices.Queries
{
    public record GetRoleOptionsQuery : IRequest<ApiResponses<IEnumerable<RoleOptionDto>>>;

    public class GetRoleOptionsQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetRoleOptionsQuery, ApiResponses<IEnumerable<RoleOptionDto>>>
    {
        public async Task<ApiResponses<IEnumerable<RoleOptionDto>>> Handle(GetRoleOptionsQuery request, CancellationToken cancellationToken)
        {
            var rolesRepo = unitOfWork.GetRepository<Domain.Entities.Roles>();

            var options = await rolesRepo.GetByCriteriaQueryable(r => r.IsActive)
                .AsNoTracking()
                .Select(r => new RoleOptionDto
                {
                    Label = r.NameEn,
                    Value = r.Id
                })
                .OrderBy(r => r.Label)
                .ToListAsync(cancellationToken);

            return ApiResponses<IEnumerable<RoleOptionDto>>.Success(options, "Role options retrieved successfully.");
        }
    }
}