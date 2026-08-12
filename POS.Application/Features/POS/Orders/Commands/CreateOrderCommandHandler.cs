using MediatR;
using Pos.Application.Common.DTOs;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace Pos.Application.Features.Orders.Commands
{
    public class CreateOrderCommand : IRequest<ApiResponses<string>>
    {
        public List<OrderDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
    }
    public class CreateOrderCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<CreateOrderCommand, ApiResponses<string>>
    {


        public async Task<ApiResponses<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
   

            var orderRepo = unitOfWork.GetRepository<Order>();
            var neworder = AppMapper.Mapper.Map<Order>(request.Items);

            // 2. Apply business rules explicitly
            neworder.OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}";
            neworder.Status = OrderStatus.Paid;
            await orderRepo.Insert(neworder);
            await unitOfWork.DoWork();
            
            // Note: In a full event-driven system, you would raise an OrderCompletedEvent here
            // to trigger the inventory deduction asynchronously.
            
            return ApiResponses<string>.Success(neworder.OrderNumber, "Order created successfully.");
        }
    }
}