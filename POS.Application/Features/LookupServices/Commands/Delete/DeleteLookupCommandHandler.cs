using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.LookupServices.Commands
{
    public record DeleteLookupCommand(long id) : IRequest<ApiResponses<bool>>;

    public class DeleteLookupCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork) : IRequestHandler<DeleteLookupCommand, ApiResponses<bool>>
    {

        public async Task<ApiResponses<bool>> Handle(DeleteLookupCommand request, CancellationToken cancellationToken)
        {
            var lookupRepo = unitOfWork.GetRepository<Lookup>();
            var lookupData = await lookupRepo.GetFirstOrDefault<Lookup>(null, x => x.Id == request.id, null, null, true);

            if (lookupData == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Lookup item not found.");
            }

            await lookupRepo.Delete(lookupData);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Lookup item deleted successfully.");
        }
    }
}
