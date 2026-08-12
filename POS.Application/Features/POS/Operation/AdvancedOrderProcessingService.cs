using MediatR;
using Microsoft.EntityFrameworkCore;
using Pos.Application.Features.Orders.Commands;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace Pos.Application.Features.Operations
{
    // This replaces the basic CreateOrderCommandHandler to handle Tax, Subtotals, and BOM Inventory Deductions.
    public class AdvancedOrderProcessingHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<CreateOrderCommand, ApiResponses<long>>
    {
        private const decimal VatRate = 0.14m; // Standard 14% VAT configuration
        public async Task<ApiResponses<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Calculate Subtotals and VAT securely on the backend
            decimal calculatedSubtotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var itemDto in request.Items)
            {
                var product = await _unitOfWork.Context.Products
                    .Include(p => p.RecipeItems) // Load BOM if composite
                    .FirstOrDefaultAsync(p => p.Id == itemDto.ProductId, cancellationToken);

                if (product == null) continue;

                decimal lineTotal = product.Price * itemDto.Quantity;
                calculatedSubtotal += lineTotal;

                orderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                });

                // 2. Perform Inventory Deductions based on BOM (Bill of Materials) or Direct Link
                if (product.TrackInventory)
                {
                    if (product.IsComposite && product.RecipeItems.Any())
                    {
                        // Deduct ingredients (e.g., 15g coffee, 50ml milk for Espresso)
                        foreach (var recipeItem in product.RecipeItems)
                        {
                            await DeductStockAsync(recipeItem.InventoryItemId, recipeItem.QuantityRequired * itemDto.Quantity, cancellationToken);
                        }
                    }
                    else if (product.LinkedInventoryItemId.HasValue)
                    {
                        // Deduct 1-to-1 item (e.g., Bottled Water)
                        await DeductStockAsync(product.LinkedInventoryItemId.Value, itemDto.Quantity, cancellationToken);
                    }
                }
            }

            // Apply Tax Calculation
            decimal taxAmount = calculatedSubtotal * VatRate;
            decimal finalTotal = calculatedSubtotal + taxAmount;

            // 3. Create the Order Record
            var order = new Order
            {
                Id = Guid.NewGuid(),
                OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}",
                TotalAmount = finalTotal, // Use backend calculation to prevent frontend manipulation
                Status = OrderStatus.Paid,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            return order.Id;
        }

        private async Task DeductStockAsync(Guid inventoryItemId, decimal amountToDeduct, CancellationToken cancellationToken)
        {
            var inventoryItem = await _context.InventoryItems.FindAsync(new object[] { inventoryItemId }, cancellationToken);
            if (inventoryItem != null)
            {
                inventoryItem.CurrentStock -= amountToDeduct;

                _context.StockTransactions.Add(new StockTransaction
                {
                    Id = Guid.NewGuid(),
                    InventoryItemId = inventoryItemId,
                    QuantityChanged = -amountToDeduct,
                    Type = TransactionType.Sale,
                    Remarks = "System deduction for sale"
                });
            }
        }
    }
}