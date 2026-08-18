using MediatR;
using POS.Domain.Events;

namespace POS.Application.Features.POS.Orders.Notifications
{
    public record OrderPaidNotification(OrderPaidDomainEvent Event) : INotification;

    public record OrderCreatedNotification(OrderCreatedDomainEvent Event) : INotification;
}
