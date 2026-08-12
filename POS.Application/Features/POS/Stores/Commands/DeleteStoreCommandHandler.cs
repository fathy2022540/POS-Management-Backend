using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Stores.Commands
{
    public record DeleteStoreCommand(long id) : IRequest<ApiResponses<bool>>;
    public class DeleteStoreCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<DeleteStoreCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(DeleteStoreCommand request, CancellationToken cancellationToken)
        {
            var storeRepo = unitOfWork.GetRepository<Store>();
            var storeData = await storeRepo.GetFirstOrDefault<Store>(null, x => x.Id == request.id, null, null, true);

            if (storeData == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Store item not found.");
            }

            await storeRepo.Delete(storeData);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Store item deleted successfully.");
        }
    }
}