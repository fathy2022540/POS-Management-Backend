using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.RolesServices.Commands
{
    public record UpdateRoleCommand(long Id, string NameEN, string? NameAR,string Code,string? Description,bool IsActive) : IRequest<ApiResponses<bool>>;

    public class UpdateRoleCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<UpdateRoleCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var rolesRepo = unitOfWork.GetRepository<Roles>();

            // 1. Fetch tracked entity
            var existingRole = await rolesRepo.Find(request.Id);
            if (existingRole == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Role not found.");
            }

            // 3. Map properties directly onto tracked entity
            AppMapper.Mapper.Map(request, existingRole);

            await rolesRepo.Update(existingRole);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Role updated successfully.");
        }
    }
}
