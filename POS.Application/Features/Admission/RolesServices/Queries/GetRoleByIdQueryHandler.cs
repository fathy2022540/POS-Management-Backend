using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.RolesServices.Queries
{
    public class GetRoleByIdQuery : IRequest<ApiResponses<RoleDto>>
    {
        public long Id { get; set; }
    }

    public class GetRoleByIdQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetRoleByIdQuery, ApiResponses<RoleDto>>
    {
        public async Task<ApiResponses<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var rolesRepo = unitOfWork.GetRepository<Roles>();

            var role = await rolesRepo.Find(request.Id);
            if (role == null)
            {
                return ApiResponses<RoleDto>.Failure(StatusResult.NotFound, "Role not found.");
            }

            var mappedRole = AppMapper.Mapper.Map<RoleDto>(role);
            return ApiResponses<RoleDto>.Success(mappedRole, "Role retrieved successfully.");
        }
    }
}