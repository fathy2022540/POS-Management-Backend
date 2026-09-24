using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Admission.PermissionServices.Queries
{
    public record GetUserOptionsQuery : IRequest<ApiResponses<IEnumerable<UserOptionDto>>>;

    public class GetUserOptionsQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetUserOptionsQuery, ApiResponses<IEnumerable<UserOptionDto>>>
    {
        public async Task<ApiResponses<IEnumerable<UserOptionDto>>> Handle(GetUserOptionsQuery request, CancellationToken cancellationToken)
        {
            var usersRepo = unitOfWork.GetRepository<Users>();

            var users = await usersRepo.GetByCriteriaQueryable(u => u.IsActive == true)
                .AsNoTracking()
                .Select(u => new UserOptionDto
                {
                    Label = u.FullName + " (" + u.UserName + ")",
                    Value = u.Id
                })
                .OrderBy(u => u.Label)
                .ToListAsync(cancellationToken);

            return ApiResponses<IEnumerable<UserOptionDto>>.Success(users, "User options retrieved successfully.");
        }
    }
}