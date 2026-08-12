using System;
using POS.Domain.Enums;

namespace POS.Domain.Entities
{
    public class StockTransaction : BaseEntity
    {
        public long InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; } = null!;

        public decimal QuantityChanged { get; set; }
        public TransactionType Type { get; set; }
        
        public long? ReferenceOrderId { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}