using MediatR;
using POS.Application.Common.DTOs;
using POS.Domain.Enums;
using POS.Domain.Repositories;

namespace POS.Application.Features.POS.Orders.Queries
{
    public record GetDailySalesQuery(DateTime Date) : IRequest<ApiResponses<decimal>>;

    public class GetDailySalesQueryHandler(IPosUnitOfWork unitOfWork)
        : IRequestHandler<GetDailySalesQuery, ApiResponses<decimal>>
    {
        public async Task<ApiResponses<decimal>> Handle(GetDailySalesQuery request, CancellationToken cancellationToken)
        {
            var totalSales = await unitOfWork.Orders.GetDailySalesTotalAsync(request.Date, cancellationToken);

            if (totalSales == 0)
            {
                return ApiResponses<decimal>.Failure(StatusResult.NotFound, "No orders found for the specified date.");
            }

            return ApiResponses<decimal>.Success(
                totalSales,
                $"Daily sales for {request.Date:yyyy-MM-dd} calculated successfully.");
        }
    }
}
