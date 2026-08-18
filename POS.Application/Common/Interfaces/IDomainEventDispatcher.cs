using POS.Domain.Common;

namespace POS.Application.Common.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default);
    }
}
