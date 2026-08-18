using MediatR;
using POS.Application.Features.POS.Orders.Notifications;
using POS.Domain.Repositories;

namespace POS.Application.Features.POS.Orders.EventHandlers
{
    /// <summary>
    /// Reacts to OrderPaid domain event by deducting inventory across the Inventory aggregate boundary.
    /// </summary>
    public class OrderPaidNotificationHandler(IPosUnitOfWork unitOfWork)
        : INotificationHandler<OrderPaidNotification>
    {
        public async Task Handle(OrderPaidNotification notification, CancellationToken cancellationToken)
        {
            var orderEvent = notification.Event;

            foreach (var line in orderEvent.Lines)
            {
                var product = await unitOfWork.Products.GetByIdWithRecipeAsync(line.ProductId, cancellationToken);
                if (product is null || !product.TrackInventory)
                    continue;

                if (product.IsComposite && product.RecipeItems.Count > 0)
                {
                    foreach (var recipe in product.RecipeItems)
                    {
                        await DeductAsync(
                            recipe.InventoryItemId,
                            recipe.QuantityRequired * line.Quantity,
                            orderEvent.OrderId,
                            cancellationToken);
                    }
                }
                else if (product.LinkedInventoryItemId.HasValue)
                {
                    await DeductAsync(
                        product.LinkedInventoryItemId.Value,
                        line.Quantity,
                        orderEvent.OrderId,
                        cancellationToken);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task DeductAsync(
            long inventoryItemId,
            decimal quantity,
            long orderId,
            CancellationToken cancellationToken)
        {
            var inventoryItem = await unitOfWork.Inventory.GetByIdAsync(inventoryItemId, cancellationToken);
            if (inventoryItem is null)
                return;

            inventoryItem.DeductStock(quantity, orderId, "Sale deduction");
            await unitOfWork.Inventory.UpdateAsync(inventoryItem, cancellationToken);
        }
    }
}
