using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.DTOs.POS;
using POS.Application.Common.Interfaces;
using POS.Domain.Exceptions;
using POS.Domain.Repositories;
using POS.Domain.ValueObjects;

namespace POS.Application.Features.POS.Orders.Commands
{
    public class CreateOrderCommand : IRequest<ApiResponses<CreateOrderResultDto>>
    {
        public List<OrderItemDto> Items { get; set; } = [];
    }

    public class CreateOrderCommandHandler(
        IPosUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher)
        : IRequestHandler<CreateOrderCommand, ApiResponses<CreateOrderResultDto>>
    {
        public async Task<ApiResponses<CreateOrderResultDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var lines = new List<(long ProductId, int Quantity, Money UnitPrice)>();

                foreach (var item in request.Items)
                {
                    var product = await unitOfWork.Products.GetByIdAsync(item.ProductId, cancellationToken);
                    if (product is null)
                    {
                        return ApiResponses<CreateOrderResultDto>.Failure(
                            Domain.Enums.StatusResult.NotFound,
                            $"Product with id {item.ProductId} was not found.");
                    }

                    lines.Add((product.Id, item.Quantity, new Money(product.Price)));
                }

                var order = Domain.Entities.Order.Create(OrderNumber.Generate(), lines);

                await unitOfWork.Orders.AddAsync(order, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                order.NotifyCreated();
                await domainEventDispatcher.DispatchEventsAsync(order, cancellationToken);

                var result = new CreateOrderResultDto
                {
                    OrderId = order.Id,
                    OrderNumber = order.OrderNumber,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status.ToString()
                };

                return ApiResponses<CreateOrderResultDto>.Success(result, "Order created successfully. Awaiting payment.");
            }
            catch (DomainException ex)
            {
                return ApiResponses<CreateOrderResultDto>.Failure(Domain.Enums.StatusResult.BadRequest, ex.Message);
            }
        }
    }
}
