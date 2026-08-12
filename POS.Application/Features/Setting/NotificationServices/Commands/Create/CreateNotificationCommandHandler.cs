using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.NotificationServices.Commands
{
    public record CreateNotificationCommand(
        long UserId,
        string TitleEn,
        string TitleAr,
        string MessageEn,
        string MessageAr,
        string? ReferenceType,
        long? ReferenceId,
        string? ActionUrl) : IRequest<ApiResponses<bool>>;

    public class CreateNotificationCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<CreateNotificationCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var usersRepo = unitOfWork.GetRepository<Users>();
            var notificationRepo = unitOfWork.GetRepository<Notification>();

            var userExists = await usersRepo.AnyAsync(x => x.Id == request.UserId && !x.Is_Deleted);
            if (!userExists)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "User not found.");
            }

            var notification = new Notification
            {
                UserId = request.UserId,
                TitleEn = request.TitleEn.Trim(),
                TitleAr = request.TitleAr.Trim(),
                MessageEn = request.MessageEn.Trim(),
                MessageAr = request.MessageAr.Trim(),
                ReferenceType = request.ReferenceType?.Trim(),
                ReferenceId = request.ReferenceId,
                ActionUrl = request.ActionUrl?.Trim(),
                IsRead = false
            };

            await notificationRepo.Insert(notification);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Notification created successfully.");
        }
    }
}