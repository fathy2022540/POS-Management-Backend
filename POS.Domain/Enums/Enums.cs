namespace POS.Domain.Enums
{
    public enum OrderStatus { Pending=1, Paid=2, Cancelled=3, Refunded=4 }
    public enum TransactionType { Sale=1, Restock=2, Adjustment=3, Spoilage=4 }
}
