namespace POS.Domain.Exceptions
{
    public class InsufficientStockException : DomainException
    {
        public InsufficientStockException(string itemName, decimal available, decimal requested)
            : base($"Insufficient stock for '{itemName}'. Available: {available}, requested: {requested}.")
        {
        }
    }
}
