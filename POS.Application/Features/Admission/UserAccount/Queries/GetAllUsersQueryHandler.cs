using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.UsersAccount.Queries
{
    public class GetAllUsersQuery : Pagination, IRequest<ApiResponses<PaginationResponse<UserDto>>>
    {
        public string? SearchTerm { get; set; }
        public long? Status { get; set; }
        public long? RoleId { get; set; }
    }

    public class GetAllUsersQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetAllUsersQuery, ApiResponses<PaginationResponse<UserDto>>>
    {
        public async Task<ApiResponses<PaginationResponse<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userRepo = unitOfWork.GetRepository<Users>();

                var queryable = userRepo.GetAllQeryable();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.Trim();
                    queryable = queryable.Where(x =>
                        (x.Email != null && x.Email.Contains(searchTerm)) ||
                        (x.FullName != null && x.FullName.Contains(searchTerm)) ||
                        (x.UserName != null && x.UserName.Contains(searchTerm)));
                }

                if (request.Status.HasValue)
                    queryable = queryable.Where(x => x.StatusId == request.Status);

                if (request.RoleId.HasValue)
                    queryable = queryable.Where(x => x.RoleId == request.RoleId);

                var totalCount = await queryable.CountAsync(cancellationToken);

                var users = await queryable
                    .OrderBy(u => u.FullName)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

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
                return ApiResponses<PaginationResponse<UserDto>>.Failure(StatusResult.InvalidRequest, $"Error retrieving users: {ex.Message}");
            }
        }
    }
}