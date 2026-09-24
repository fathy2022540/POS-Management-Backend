using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.RolesServices.Queries
{
    public class GetAllRolesQuery : Pagination, IRequest<ApiResponses<PaginationResponse<RoleDto>>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }

    public class GetAllRolesQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetAllRolesQuery, ApiResponses<PaginationResponse<RoleDto>>>
    {
        public async Task<ApiResponses<PaginationResponse<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var rolesRepo = unitOfWork.GetRepository<Roles>();

                var queryable = rolesRepo.GetByCriteriaQueryable(x =>
                    (string.IsNullOrEmpty(request.Search) || x.NameEn.Contains(request.Search) || (x.NameAr != null && x.NameAr.Contains(request.Search))) &&
                    (!request.IsActive.HasValue || x.IsActive == request.IsActive.Value));

                var totalCount = await queryable.CountAsync(cancellationToken);

                var roles = await queryable
                    .OrderBy(x => x.NameEn)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var mappedRoles = AppMapper.Mapper.Map<IEnumerable<RoleDto>>(roles);

                var response = new PaginationResponse<RoleDto>
                {
                    Count = totalCount,
                    Data = mappedRoles
                };

                return ApiResponses<PaginationResponse<RoleDto>>.Success(response, "Roles retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponses<PaginationResponse<RoleDto>>.Failure(StatusResult.InternalServerError, $"Error retrieving roles: {ex.Message}");
            }
        }
    }
}