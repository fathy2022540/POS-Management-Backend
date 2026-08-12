using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Inventory.Commands
{
    public record TransferInventoryCommand(long SourceStoreId, long DestinationStoreId, long InventoryItemId, decimal Quantity) : IRequest<ApiResponses<bool>>;
    public class TransferInventoryCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<TransferInventoryCommand, ApiResponses<bool>>
    {


        public async Task<ApiResponses<bool>> Handle(TransferInventoryCommand request, CancellationToken cancellationToken)
        {
            // Note: This assumes InventoryItem is scoped per Store.
            // You would typically query the source store's inventory to verify stock,
            // deduct it, and then add it to the destination store's inventory.

            var InventoryRepo = unitOfWork.GetRepository<InventoryTransfer>();
            var transfer = new InventoryTransfer
            {
                SourceStoreId = request.SourceStoreId,
                DestinationStoreId = request.DestinationStoreId,
                InventoryItemId = request.InventoryItemId,
                Quantity = request.Quantity,
                Status = "Completed"
            };

            // Implement stock deduction from Source and addition to Destination here

            await InventoryRepo.Insert(transfer);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Stock adjusted successfully.");
        }
    }
}