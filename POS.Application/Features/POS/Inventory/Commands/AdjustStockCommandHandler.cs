using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.Repositories;

namespace POS.Application.Features.POS.Inventory.Commands
{
    public record AdjustStockCommand(CreateStockDto CreateStockDto) : IRequest<ApiResponses<bool>>;

    public class AdjustStockCommandHandler(IPosUnitOfWork unitOfWork)
        : IRequestHandler<AdjustStockCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var item = await unitOfWork.Inventory.GetByIdAsync(request.CreateStockDto.InventoryItemId, cancellationToken);
                if (item is null)
                    return ApiResponses<bool>.Failure(StatusResult.NotFound, "Inventory item not found.");

                item.AdjustStock(
                    request.CreateStockDto.QuantityChanged,
                    request.CreateStockDto.Type,
                    request.CreateStockDto.Remarks);

                await unitOfWork.Inventory.UpdateAsync(item, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponses<bool>.Success(true, "Stock adjusted successfully.");
            }
            catch (DomainException ex)
            {
                return ApiResponses<bool>.Failure(StatusResult.BadRequest, ex.Message);
            }
        }
    }
}
