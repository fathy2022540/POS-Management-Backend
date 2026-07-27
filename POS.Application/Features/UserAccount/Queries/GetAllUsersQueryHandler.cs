// JRM.Application/Features/Users/Queries/GetAllUsers/GetAllUsersQueryHandler.cs
using MediatR;
using Microsoft.EntityFrameworkCore;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.UsersAccount.Queries
{
    public class GetAllUsersQuery : Pagination, IRequest<ApiResponses<PaginationResponse<UserDto>>>
    {
        public string? SearchTerm { get; set; }
        public long? Status { get; set; }
        public long? RoleId { get; set; }
    }
    public class GetAllUsersQueryHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<GetAllUsersQuery, ApiResponses<PaginationResponse<UserDto>>>
    {
        public async Task<ApiResponses<PaginationResponse<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userRepo = unitOfWork.GetRepository<Users>();

                var queryable = userRepo.GetByCriteriaQueryable(x => true);

                if (!string.IsNullOrEmpty(request.SearchTerm))
                    queryable = queryable.Where(x => 
                        x.Email.Contains(request.SearchTerm) || 
                        x.FullName.Contains(request.SearchTerm));

                if (request.Status.HasValue)
                    queryable = queryable.Where(x => x.StatusId == request.Status);

                var totalCount = await queryable.CountAsync(cancellationToken);

                var users = await userRepo.GetList(
                    predicate: x => (string.IsNullOrEmpty(request.SearchTerm) || 
                                   x.Email.Contains(request.SearchTerm) || 
                                   x.FullName.Contains(request.SearchTerm)) &&
                                  (!request.Status.HasValue || x.StatusId == request.Status),
                    orderBy: x => x.OrderBy(u => u.FullName),
                    include: null,
                    disableTracking: true,
                    skip: (request.PageNumber - 1) * request.PageSize,
                    take: request.PageSize
                );

                var mappedUsers = AppMapper.Mapper.Map<IEnumerable<UserDto>>(users);

                var response = new PaginationResponse<UserDto>
                {
                    Count = totalCount,
                    Data = mappedUsers
                };

                return ApiResponses<PaginationResponse<UserDto>>.Success(response, "Users retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponses<PaginationResponse<UserDto>>.Failure(StatusResult.InvalidRequest,$"Error retrieving users: {ex.Message}");
            }
        }
    }
}