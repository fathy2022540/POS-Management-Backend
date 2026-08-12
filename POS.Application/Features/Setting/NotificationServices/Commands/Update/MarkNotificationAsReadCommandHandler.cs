using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.NotificationServices.Commands
{
    public record MarkNotificationAsReadCommand(long NotificationId, long UserId) : IRequest<ApiResponses<bool>>;

    public class MarkNotificationAsReadCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<MarkNotificationAsReadCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notificationRepo = unitOfWork.GetRepository<Notification>();
            var notification = await notificationRepo.Find(request.NotificationId);

            if (notification == null || notification.Is_Deleted || notification.UserId != request.UserId)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Notification not found.");
            }

            if (notification.IsRead)
            {
                return ApiResponses<bool>.Success(true, "Notification already marked as read.");
            }

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;

            await notificationRepo.Update(notification);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Notification marked as read.");
        }
    }
}
