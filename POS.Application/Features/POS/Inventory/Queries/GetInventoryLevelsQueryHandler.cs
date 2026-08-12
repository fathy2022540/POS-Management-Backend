using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Inventory.Queries
{
    public class GetInventoryLevelsQuery : Pagination, IRequest<ApiResponses<PaginationResponse<InventoryLevelDto>>>
    {
    }
    public class GetInventoryLevelsQueryHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<GetInventoryLevelsQuery, ApiResponses<PaginationResponse<InventoryLevelDto>>>
    {

        public async Task<ApiResponses<PaginationResponse<InventoryLevelDto>>> Handle(GetInventoryLevelsQuery request, CancellationToken cancellationToken)
        {

            // Note: Adjust 'Lookups' to match your actual Domain Entity name
            var InventoryRepo = unitOfWork.GetRepository<InventoryItem>();

            var totalCount = await InventoryRepo.GetByCriteriaQueryable(x => true).CountAsync(cancellationToken); // Tracking is not needed for read-only queries

            var Inventory = await InventoryRepo.GetList(null, null, null,
                    disableTracking: true, // Tracking is not needed for read-only queries
                    skip: (request.PageNumber - 1) * request.PageSize,
                    take: request.PageSize
                    );

            var mappedInventory = AppMapper.Mapper.Map<IEnumerable<InventoryLevelDto>>(Inventory);

            var response = new PaginationResponse<InventoryLevelDto>
            {
                Count = totalCount,
                Data = mappedInventory
            };

            return ApiResponses<PaginationResponse<InventoryLevelDto>>.Success(response, "Inventory levels retrieved successfully.");

        }
    }
}