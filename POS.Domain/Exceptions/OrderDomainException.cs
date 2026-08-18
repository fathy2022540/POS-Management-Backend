namespace POS.Domain.Exceptions
{
    public class OrderDomainException : DomainException
    {
        public OrderDomainException(string message) : base(message)
        {
        }
    }
}
