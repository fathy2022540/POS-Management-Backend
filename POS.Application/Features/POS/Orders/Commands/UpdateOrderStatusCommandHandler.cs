using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;


namespace Pos.Application.Features.Orders.Commands
{
    public record UpdateOrderStatusCommand(long id, OrderStatus NewStatus) : IRequest<ApiResponses<bool>>;

    public class UpdateOrderStatusCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<UpdateOrderStatusCommand, ApiResponses<bool>>
    {


        public async Task<ApiResponses<bool>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderRepo = unitOfWork.GetRepository<Order>();


            var existingOrder = await orderRepo.Find(request.id);
            if (existingOrder == null)
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Order not found.");

            existingOrder.Status = request.NewStatus;

            await orderRepo.Update(existingOrder);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Order updated successfully.");

        }
    }
}