using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Common.Helper;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.RolesServices.Commands
{
    public record CreateRoleCommand(string? Code,string NameEN,string? NameAR, string? Description,bool Active) : IRequest<ApiResponses<bool>>;

    public class CreateRoleCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<CreateRoleCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var rolesRepo = unitOfWork.GetRepository<Roles>();

            // Optional: Check if role name already exists to prevent duplicates
            var roleExists = await BusinessValidator.FindConflictAsync(rolesRepo, v => v.NameEn.ToLower() == request.NameEN.ToLower());
            if (roleExists != null)
            {
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "Role name already exists.");
            }

            // Map request to new Roles entity instance
            var newRole = AppMapper.Mapper.Map<Roles>(request);

            await rolesRepo.Insert(newRole);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Role created successfully.");
        }
    }
}