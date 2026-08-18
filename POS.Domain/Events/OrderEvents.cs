using POS.Domain.Common;

namespace POS.Domain.Events
{
    public record OrderLineSnapshot(long ProductId, int Quantity);

    public record OrderCreatedDomainEvent(long OrderId, string OrderNumber) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }

    public record OrderPaidDomainEvent(
        long OrderId,
        string OrderNumber,
        IReadOnlyList<OrderLineSnapshot> Lines) : IDomainEvent
    {
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
