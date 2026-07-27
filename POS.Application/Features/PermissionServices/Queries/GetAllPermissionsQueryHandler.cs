// JRM.Application/Features/Permissions/Queries/GetAllPermissions/GetAllPermissionsQueryHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.PermissionServices.Queries
{
    public class GetAllPermissionsQuery : Pagination, IRequest<ApiResponses<PaginationResponse<PermissionDto>>>
    {
        public string? Resource { get; set; }
        public string? Action { get; set; }
    }
    public class GetAllPermissionsQueryHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<GetAllPermissionsQuery, ApiResponses<PaginationResponse<PermissionDto>>>
    {
        public async Task<ApiResponses<PaginationResponse<PermissionDto>>> Handle(GetAllPermissionsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var permissionRepo = unitOfWork.GetRepository<Permissions>();

                var queryable = permissionRepo.GetByCriteriaQueryable(x => true);

                if (!string.IsNullOrEmpty(request.Resource))
                    queryable = queryable.Where(x => x.Resource == request.Resource);

                if (!string.IsNullOrEmpty(request.Action))
                    queryable = queryable.Where(x => x.Action == request.Action);

                var totalCount = await queryable.CountAsync(cancellationToken);

                var permissions = await permissionRepo.GetList(
                    predicate: x => (string.IsNullOrEmpty(request.Resource) || x.Resource == request.Resource) &&
                                  (string.IsNullOrEmpty(request.Action) || x.Action == request.Action),
                    orderBy: x => x.OrderBy(p => p.Resource).ThenBy(p => p.Action),
                    include: null,
                    disableTracking: true,
                    skip: (request.PageNumber - 1) * request.PageSize,
                    take: request.PageSize
                );

                var mappedPermissions = AppMapper.Mapper.Map<IEnumerable<PermissionDto>>(permissions);

                var response = new PaginationResponse<PermissionDto>
                {
                    Count = totalCount,
                    Data = mappedPermissions
                };

                return ApiResponses<PaginationResponse<PermissionDto>>.Success(response, "Permissions retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponses<PaginationResponse<PermissionDto>>.Failure(StatusResult.ModelNotValid, $"Error retrieving permissions: {ex.Message}");
            }
        }
    }
}