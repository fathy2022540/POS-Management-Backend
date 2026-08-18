using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.DTOs.POS;
using POS.Application.Helper.Mapper;
using POS.Domain.Repositories;

namespace POS.Application.Features.POS.Orders.Queries
{
    public class GetOrdersQuery : Pagination, IRequest<ApiResponses<PaginationResponse<OrderDto>>>
    {
    }

    public class GetOrdersQueryHandler(IPosUnitOfWork unitOfWork)
        : IRequestHandler<GetOrdersQuery, ApiResponses<PaginationResponse<OrderDto>>>
    {
        public async Task<ApiResponses<PaginationResponse<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var totalCount = await unitOfWork.Orders.CountAsync(cancellationToken);

            var orders = await unitOfWork.Orders.GetPagedAsync(
                (request.PageNumber - 1) * request.PageSize,
                request.PageSize,
                cancellationToken);

            var mappedOrders = AppMapper.Mapper.Map<IEnumerable<OrderDto>>(orders);

            var response = new PaginationResponse<OrderDto>
            {
                Count = totalCount,
                Data = mappedOrders,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return ApiResponses<PaginationResponse<OrderDto>>.Success(response, "Orders retrieved successfully.");
        }
    }
}
