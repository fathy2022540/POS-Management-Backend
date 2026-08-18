using MediatR;
using POS.Application.Features.POS.Orders.Notifications;

namespace POS.Application.Features.POS.Orders.EventHandlers
{
    public class OrderCreatedNotificationHandler : INotificationHandler<OrderCreatedNotification>
    {
        public Task Handle(OrderCreatedNotification notification, CancellationToken cancellationToken)
        {
            // Extension point: send receipt, notify kitchen display, audit log, etc.
            return Task.CompletedTask;
        }
    }
}
