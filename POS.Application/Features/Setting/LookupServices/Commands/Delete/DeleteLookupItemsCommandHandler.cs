using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.LookupServices.Commands
{
    public record DeleteLookupItemsCommand(long id) : IRequest<ApiResponses<bool>>;

    public class DeleteLookupItemsCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<DeleteLookupItemsCommand, ApiResponses<bool>>
    {

        public async Task<ApiResponses<bool>> Handle(DeleteLookupItemsCommand request, CancellationToken cancellationToken)
        {
            var lookupRepo = unitOfWork.GetRepository<LookupItems>();
            var lookupItemsData = await lookupRepo.GetFirstOrDefault<LookupItems>(null, x => x.Id == request.id, null, null, true);

            if (lookupItemsData == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Lookup item not found.");
            }

            await lookupRepo.Delete(lookupItemsData);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Lookup item deleted successfully.");
        }
    }
}
