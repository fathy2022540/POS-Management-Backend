using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.NotificationServices.Commands
{
    public record MarkAllNotificationsAsReadCommand(long UserId) : IRequest<ApiResponses<bool>>;

    public class MarkAllNotificationsAsReadCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<MarkAllNotificationsAsReadCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var notificationRepo = unitOfWork.GetRepository<Notification>();
            var notifications = await notificationRepo.GetList(
                predicate: x => x.UserId == request.UserId && !x.Is_Deleted && !x.IsRead,
                orderBy: null,
                include: null,
                disableTracking: false);

            if (notifications.Count == 0)
            {
                return ApiResponses<bool>.Success(true, "No unread notifications found.");
            }

            var utcNow = DateTime.UtcNow;
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = utcNow;
            }

            await notificationRepo.UpdateRange(notifications);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Notifications marked as read.");
        }
    }
}