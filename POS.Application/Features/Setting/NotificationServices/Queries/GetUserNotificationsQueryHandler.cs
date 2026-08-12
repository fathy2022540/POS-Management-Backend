using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.NotificationServices.Queries
{
    public record GetUserNotificationsQuery(long UserId, int Take = 10) : IRequest<ApiResponses<UserNotificationsResponse>>;

    public class GetUserNotificationsQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetUserNotificationsQuery, ApiResponses<UserNotificationsResponse>>
    {
        public async Task<ApiResponses<UserNotificationsResponse>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notificationRepo = unitOfWork.GetRepository<Notification>();

            var unreadCount = await notificationRepo.CountAsync(x => x.UserId == request.UserId && !x.Is_Deleted && !x.IsRead);

            var items = await notificationRepo.GetListWithSelect(
                selector: x => new NotificationItemResponse
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    TitleEn = x.TitleEn,
                    TitleAr = x.TitleAr,
                    MessageEn = x.MessageEn,
                    MessageAr = x.MessageAr,
                    ReferenceType = x.ReferenceType,
                    ReferenceId = x.ReferenceId,
                    ActionUrl = x.ActionUrl,
                    IsRead = x.IsRead,
                    CreatedDate = x.CreatedDate,
                    ReadAt = x.ReadAt
                },
                predicate: x => x.UserId == request.UserId && !x.Is_Deleted,
                orderBy: q => q.OrderByDescending(x => x.CreatedDate),
                include: null,
                disableTracking: true,
                skip: 0,
                take: request.Take > 0 ? request.Take : 10);

            var response = new UserNotificationsResponse
            {
                UnreadCount = unreadCount,
                Items = items
            };

            return ApiResponses<UserNotificationsResponse>.Success(response);
        }
    }
}