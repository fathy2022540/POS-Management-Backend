// POS.Application/Features/Users/Queries/GetUserById/GetUserByIdQueryHandler.cs
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
    public class GetUserByIdQuery : IRequest<ApiResponses<UserDetailDto>>
    {
        public long UserId { get; set; }
    }
    public class GetUserByIdQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetUserByIdQuery, ApiResponses<UserDetailDto>>
    {
        public async Task<ApiResponses<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userRepo = unitOfWork.GetRepository<Users>();

                var user = await userRepo.GetByCriteriaQueryable(x => x.Id == request.UserId)
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(cancellationToken);

                if (user == null)
                    return ApiResponses<UserDetailDto>.Failure(StatusResult.NotFound, "User not found.");

                var mappedUser = AppMapper.Mapper.Map<UserDetailDto>(user);
                return ApiResponses<UserDetailDto>.Success(mappedUser, "User retrieved successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponses<UserDetailDto>.Failure(StatusResult.InvalidRequest,$"Error retrieving user: {ex.Message}");
            }
        }
    }
}