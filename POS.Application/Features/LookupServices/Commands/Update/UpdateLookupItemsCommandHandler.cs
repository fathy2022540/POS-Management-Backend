using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.LookupServices.Commands
{
    public record UpdateLookupItemsCommand(long id, string Name, string NameAR, string Code) : IRequest<ApiResponses<bool>>;

    public class UpdateLookupItemsCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork) : IRequestHandler<UpdateLookupItemsCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateLookupItemsCommand request, CancellationToken cancellationToken)
        {
            var lookupItemsRepo = unitOfWork.GetRepository<LookupItems>();


            var existingLookupItem = await lookupItemsRepo.Find(request.id);
            if (existingLookupItem == null)
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Lookup item not found.");

            AppMapper.Mapper.Map(request, existingLookupItem);

            await lookupItemsRepo.Update(existingLookupItem);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Lookup updated successfully.");

        }
    }
}
