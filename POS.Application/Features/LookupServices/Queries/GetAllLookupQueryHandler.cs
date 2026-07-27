using MediatR;
using Microsoft.EntityFrameworkCore;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.LookupServices.Queries
{
    public class GetAllLookupQuery : Pagination, IRequest<ApiResponses<PaginationResponse<LookupDto>>>
    {
    }

    public class GetAllLookupQueryHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<GetAllLookupQuery, ApiResponses<PaginationResponse<LookupDto>>>
    {
        public async Task<ApiResponses<PaginationResponse<LookupDto>>> Handle(GetAllLookupQuery request, CancellationToken cancellationToken)
        {
            try
            {


                // Note: Adjust 'Lookups' to match your actual Domain Entity name
                var lookupRepo = unitOfWork.GetRepository<Lookup>();

                var totalCount = await lookupRepo.GetByCriteriaQueryable(x => true).CountAsync(cancellationToken);

                var lookups = await lookupRepo.GetList(null, null, null,
                    disableTracking: true, // Tracking is not needed for read-only queries
                    skip: (request.PageNumber - 1) * request.PageSize,
                    take: request.PageSize
                );

                var mappedLookups = AppMapper.Mapper.Map<IEnumerable<LookupDto>>(lookups);

                var response = new PaginationResponse<LookupDto>
                {
                    Count = totalCount,
                    Data = mappedLookups
                };

                return ApiResponses<PaginationResponse<LookupDto>>.Success(response, "Lookups retrieved successfully.");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}