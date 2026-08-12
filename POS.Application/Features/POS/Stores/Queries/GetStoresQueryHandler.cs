using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Stores.Queries
{
    public class GetStoresQuery : Pagination, IRequest<ApiResponses<PaginationResponse<StoreDto>>>
    {
        public string OrderNumber { get; set; }
    }
    public class GetStoresQueryHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<GetStoresQuery, ApiResponses<PaginationResponse<StoreDto>>>
    {
        public async Task<ApiResponses<PaginationResponse<StoreDto>>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
        {
            var storeRepo = unitOfWork.GetRepository<Store>();

            var totalCount = await storeRepo.GetByCriteriaQueryable(x => true).CountAsync(cancellationToken);
            var stores = await storeRepo.GetList(null, orderBy: o => o.OrderByDescending(x => x.CreatedDate), null,
                                        disableTracking: true, // Tracking is not needed for read-only queries
                                        skip: (request.PageNumber - 1) * request.PageSize,
                                        take: request.PageSize
                                        );

            var mappedStores = AppMapper.Mapper.Map<IEnumerable<StoreDto>>(stores);

            var response = new PaginationResponse<StoreDto>
            {
                Count = totalCount,
                Data = mappedStores
            };

            return ApiResponses<PaginationResponse<StoreDto>>.Success(response, "Stores retrieved successfully.");


        }
    }
}