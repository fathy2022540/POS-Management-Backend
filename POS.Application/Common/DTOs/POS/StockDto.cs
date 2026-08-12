using POS.Domain.Enums;
using System;

namespace POS.Application.Common.DTOs
{

    public class CreateStockDto
    {
        public long InventoryItemId { get; set; }
        public decimal QuantityChanged { get; set; }
        public TransactionType Type { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}