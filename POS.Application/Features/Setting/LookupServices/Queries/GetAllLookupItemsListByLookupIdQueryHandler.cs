using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.LookupServices.Queries
{
    public class GetAllLookupItemsListByLookupIdQuery : Pagination, IRequest<ApiResponses<PaginationResponse<LookupItemDto>>>
    {
        public long LookupId { get; set; }
    }

    public class GetAllLookupItemsListByLookupIdQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetAllLookupItemsListByLookupIdQuery, ApiResponses<PaginationResponse<LookupItemDto>>>
    {
        public async Task<ApiResponses<PaginationResponse<LookupItemDto>>> Handle(GetAllLookupItemsListByLookupIdQuery request, CancellationToken cancellationToken)
        {
            // Note: Adjust 'LookupItems' to match your actual Domain Entity name
            var lookupItemRepo = unitOfWork.GetRepository<LookupItems>();

            var totalCount = await lookupItemRepo.GetByCriteriaQueryable(li => li.LookupId == request.LookupId).CountAsync(cancellationToken);

            var lookupItems = await lookupItemRepo.GetList(
                li => li.LookupId == request.LookupId // Assumes property is named LookupId
                , c => c.OrderByDescending(c => c.Id),
                t => t?.Include(q => q.Lookup),
                disableTracking: true, // Tracking is not needed for read-only queries
                skip: (request.PageNumber - 1) * request.PageSize,
                take: request.PageSize
            );

            var mappedItems = AppMapper.Mapper.Map<IEnumerable<LookupItemDto>>(lookupItems);

            var response = new PaginationResponse<LookupItemDto>
            {
                Count = totalCount,
                Data = mappedItems
            };

            return ApiResponses<PaginationResponse<LookupItemDto>>.Success(response, "Lookup items retrieved successfully.");
        }
    }
}