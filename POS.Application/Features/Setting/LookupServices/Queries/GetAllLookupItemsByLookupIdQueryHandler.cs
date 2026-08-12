using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.LookupServices.Queries
{
    public record GetAllLookupItemsByLookupIdQuery(long LookupId) : IRequest<ApiResponses<IEnumerable<LookupItemDto>>>;
    public class GetAllLookupItemsByLookupIdQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetAllLookupItemsByLookupIdQuery, ApiResponses<IEnumerable<LookupItemDto>>>
    {
        public async Task<ApiResponses<IEnumerable<LookupItemDto>>> Handle(GetAllLookupItemsByLookupIdQuery request, CancellationToken cancellationToken)

        {
            // Note: Adjust 'LookupItems' to match your actual Domain Entity name
            var lookupItemRepo = unitOfWork.GetRepository<LookupItems>();

            var totalCount = await lookupItemRepo.GetByCriteriaQueryable(li => li.LookupId == request.LookupId).CountAsync(cancellationToken);

            var lookupItems = await lookupItemRepo.GetList(
                li => li.LookupId == request.LookupId
                , c => c.OrderByDescending(c => c.Id),
                t => t?.Include(q => q.Lookup),
                disableTracking: true
            );

            var mappedItems = AppMapper.Mapper.Map<IEnumerable<LookupItemDto>>(lookupItems);


            return ApiResponses<IEnumerable<LookupItemDto>>.Success(mappedItems, "Lookup items retrieved successfully.");
        }
    }
}