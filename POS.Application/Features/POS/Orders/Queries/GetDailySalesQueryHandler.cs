using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;


namespace Pos.Application.Features.Orders.Queries
{
    public record GetDailySalesQuery(DateTime Date) : IRequest<ApiResponses<decimal>>;

    public class GetDailySalesQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
      : IRequestHandler<GetDailySalesQuery, ApiResponses<decimal>>
    {

        public async Task<ApiResponses<decimal>> Handle(GetDailySalesQuery request, CancellationToken cancellationToken)
        {
            var startOfDay = request.Date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            var dailysalesRepo = unitOfWork.GetRepository<Order>();
            var queryable =  dailysalesRepo.GetByCriteriaQueryable(o => o.CreatedDate >= startOfDay && o.CreatedDate <= endOfDay && o.Status == OrderStatus.Paid);
            if (!queryable.Any())
            {
                return ApiResponses<decimal>.Failure(StatusResult.NotFound, "No orders found for the specified date.");
            }
            var totalSales = await queryable.SumAsync(o => o.TotalAmount, cancellationToken);


            return ApiResponses<decimal>.Success(
                            totalSales,
                            $"Daily sales for {request.Date:yyyy-MM-dd} calculated successfully."
                        );

            
        }
    }
}