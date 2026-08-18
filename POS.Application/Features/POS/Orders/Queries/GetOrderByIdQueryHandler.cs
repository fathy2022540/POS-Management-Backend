using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.DTOs.POS;
using POS.Application.Helper.Mapper;
using POS.Domain.Enums;
using POS.Domain.Repositories;

namespace POS.Application.Features.POS.Orders.Queries
{
    public record GetOrderByIdQuery(long OrderId) : IRequest<ApiResponses<OrderDto>>;

    public class GetOrderByIdQueryHandler(IPosUnitOfWork unitOfWork)
        : IRequestHandler<GetOrderByIdQuery, ApiResponses<OrderDto>>
    {
        public async Task<ApiResponses<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await unitOfWork.Orders.GetByIdWithItemsAsync(request.OrderId, cancellationToken);

            if (order is null)
                return ApiResponses<OrderDto>.Failure(StatusResult.NotFound, "Order not found.");

            var dto = AppMapper.Mapper.Map<OrderDto>(order);
            return ApiResponses<OrderDto>.Success(dto, "Order retrieved successfully.");
        }
    }
}
