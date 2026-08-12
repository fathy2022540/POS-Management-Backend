using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Inventory.Commands
{
    public record AdjustStockCommand(CreateStockDto CreateStockDto) : IRequest<ApiResponses<bool>>;
    public class AdjustStockCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<AdjustStockCommand, ApiResponses<bool>>
    {

        public async Task<ApiResponses<bool>> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
        {
            var InventoryRepo = unitOfWork.GetRepository<InventoryItem>();
            var stockTransRepo = unitOfWork.GetRepository<StockTransaction>();
            var item = await InventoryRepo.GetFirstOrDefault<InventoryItem>(selector: null,
                          predicate: x => x.Id == request.CreateStockDto.InventoryItemId,
                          orderBy: null,
                          include: null,
                          disableTracking: false
                          );
            if (item == null) return ApiResponses<bool>.Failure(StatusResult.NotFound, "Inventory item not found.");

            var transaction = new StockTransaction
            {
                InventoryItemId = request.CreateStockDto.InventoryItemId,
                QuantityChanged = request.CreateStockDto.QuantityChanged,
                Type = request.CreateStockDto.Type,
                Remarks = request.CreateStockDto.Remarks
            };

            item.CurrentStock += request.CreateStockDto.QuantityChanged;


            await stockTransRepo.Insert(transaction);
            await unitOfWork.DoWork();


            return ApiResponses<bool>.Success(true, "Stock adjusted successfully.");

        }
    }
}