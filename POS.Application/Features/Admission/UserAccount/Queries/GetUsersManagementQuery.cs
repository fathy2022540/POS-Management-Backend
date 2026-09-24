using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Application.Features.Admission.UserAccount.Queries
{
    public class GetUsersManagementQuery : IRequest<ApiResponses<IEnumerable<object>>>
    {
        public string? Search { get; set; }
    }

    public class GetUsersManagementQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetUsersManagementQuery, ApiResponses<IEnumerable<object>>>
    {
        public async Task<ApiResponses<IEnumerable<object>>> Handle(GetUsersManagementQuery request, CancellationToken cancellationToken)
        {
            var usersRepo = unitOfWork.GetRepository<Users>();
            var normalizedSearch = request.Search?.Trim();

            var query = usersRepo.GetAllQeryable();

            if (!string.IsNullOrWhiteSpace(normalizedSearch))
            {
                query = query.Where(u =>
                    (u.FullName != null && EF.Functions.Like(u.FullName, $"%{normalizedSearch}%")) ||
                    (u.Email != null && EF.Functions.Like(u.Email, $"%{normalizedSearch}%")) ||
                    (u.UserName != null && EF.Functions.Like(u.UserName, $"%{normalizedSearch}%")));
            }

            var users = await query
                .OrderByDescending(u => u.CreatedDate)
                .Select(u => new
                {
                    id = u.Id,
                    fullName = u.FullName,
                    username = u.UserName,
                    email = u.Email,
                    phone = u.Mobile,
                    roleId = u.RoleId,
                    role = u.Roles != null ? u.Roles.NameEn : string.Empty,
                    department = string.Empty,
                    isActive = u.IsActive,
                    documentName = string.Empty,
                    createdDate = u.CreatedDate
                })
                .ToListAsync(cancellationToken);

            return ApiResponses<IEnumerable<object>>.Success(users, "Users retrieved successfully.");
        }
    }
}
