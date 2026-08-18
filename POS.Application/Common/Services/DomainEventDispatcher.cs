using MediatR;
using POS.Application.Common.Interfaces;
using POS.Application.Features.POS.Orders.Notifications;
using POS.Domain.Common;
using POS.Domain.Events;

namespace POS.Application.Common.Services
{
    public class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
    {
        public async Task DispatchEventsAsync(AggregateRoot aggregate, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                INotification? notification = domainEvent switch
                {
                    OrderPaidDomainEvent paid => new OrderPaidNotification(paid),
                    OrderCreatedDomainEvent created => new OrderCreatedNotification(created),
                    _ => null
                };

                if (notification is not null)
                    await publisher.Publish(notification, cancellationToken);
            }

            aggregate.ClearDomainEvents();
        }
    }
}
