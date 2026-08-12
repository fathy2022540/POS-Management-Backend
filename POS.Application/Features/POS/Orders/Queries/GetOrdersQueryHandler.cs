using MediatR;
using Microsoft.EntityFrameworkCore;
using Pos.Application.Common.DTOs;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;


namespace Pos.Application.Features.Orders.Queries
{
    public class GetOrdersQuery : Pagination, IRequest<ApiResponses<PaginationResponse<OrderDto>>>
    {

    }
    public class GetOrdersQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetOrdersQuery, ApiResponses<PaginationResponse<OrderDto>>>
    {


        public async Task<ApiResponses<PaginationResponse<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            var orderRepo = unitOfWork.GetRepository<Order>();

            var totalCount = await orderRepo.GetByCriteriaQueryable(x => true).CountAsync(cancellationToken);

            var orders = await orderRepo.GetList(null,
                orderBy: o => o.OrderByDescending(x => x.CreatedDate),
                null,
                disableTracking: true, // Tracking is not needed for read-only queries
                skip: (request.PageNumber - 1) * request.PageSize,
                take: request.PageSize
            );

            var mappedOrders = AppMapper.Mapper.Map<IEnumerable<OrderDto>>(orders);

            var response = new PaginationResponse<OrderDto>
            {
                Count = totalCount,
                Data = mappedOrders
            };

            return ApiResponses<PaginationResponse<OrderDto>>.Success(response, "Orders retrieved successfully.");


        }
    }
}