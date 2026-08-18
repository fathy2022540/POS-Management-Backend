using POS.Domain.Common;
using POS.Domain.Enums;
using POS.Domain.Exceptions;

namespace POS.Domain.Entities
{
    /// <summary>
    /// Inventory aggregate root. All stock changes go through domain methods.
    /// </summary>
    public class InventoryItem : AggregateRoot
    {
        public string Name { get; set; } = string.Empty;
        public string UnitOfMeasure { get; set; } = string.Empty;
        public decimal CurrentStock { get; private set; }

        public ICollection<StockTransaction> Transactions { get; private set; } = [];
        public ICollection<ProductRecipeItem> AssociatedRecipes { get; set; } = [];
        public ICollection<Product> LinkedProducts { get; set; } = [];
        public ICollection<InventoryTransfer> InventoryTransfers { get; set; } = [];

        public StockTransaction DeductStock(decimal quantity, long orderId, string remarks)
        {
            if (quantity <= 0)
            {
                throw new DomainException("Deduction quantity must be greater than zero.");
            }

            if (CurrentStock < quantity)
            {
                throw new InsufficientStockException(Name, CurrentStock, quantity);
            }

            CurrentStock -= quantity;

            var transaction = new StockTransaction
            {
                InventoryItemId = Id,
                QuantityChanged = -quantity,
                Type = TransactionType.Sale,
                ReferenceOrderId = orderId,
                Remarks = remarks
            };

            Transactions.Add(transaction);
            return transaction;
        }

        public StockTransaction AdjustStock(decimal quantityChanged, TransactionType type, string remarks)
        {
            CurrentStock += quantityChanged;

            var transaction = new StockTransaction
            {
                InventoryItemId = Id,
                QuantityChanged = quantityChanged,
                Type = type,
                Remarks = remarks
            };

            Transactions.Add(transaction);
            return transaction;
        }
    }
}
