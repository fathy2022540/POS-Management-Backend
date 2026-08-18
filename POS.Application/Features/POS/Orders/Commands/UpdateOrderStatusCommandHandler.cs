using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.Repositories;

namespace POS.Application.Features.POS.Orders.Commands
{
    public record UpdateOrderStatusCommand(long Id, OrderStatus NewStatus) : IRequest<ApiResponses<bool>>;

    public class UpdateOrderStatusCommandHandler(IPosUnitOfWork unitOfWork)
        : IRequestHandler<UpdateOrderStatusCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await unitOfWork.Orders.GetByIdWithItemsAsync(request.Id, cancellationToken);
                if (order is null)
                    return ApiResponses<bool>.Failure(StatusResult.NotFound, "Order not found.");

                switch (request.NewStatus)
                {
                    case OrderStatus.Cancelled:
                        order.Cancel();
                        break;
                    case OrderStatus.Refunded:
                        order.Refund();
                        break;
                    default:
                        return ApiResponses<bool>.Failure(
                            StatusResult.BadRequest,
                            "Status changes must go through domain workflows (Cancel, Refund, or ProcessPayment).");
                }

                await unitOfWork.Orders.UpdateAsync(order, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return ApiResponses<bool>.Success(true, "Order updated successfully.");
            }
            catch (DomainException ex)
            {
                return ApiResponses<bool>.Failure(StatusResult.BadRequest, ex.Message);
            }
        }
    }
}
