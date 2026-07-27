using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JRM.Application.Features.JourneyServices.Queries
{
    public sealed record GetAllJourneysQuery() : IRequest<ApiResponses<List<JourneyDto>>>;

    public class GetAllJourneysQueryHandler(IUnitOfWork<JRMDBContext> unitOfWork)
    : IRequestHandler<GetAllJourneysQuery, ApiResponses<List<JourneyDto>>>
    {

        public async Task<ApiResponses<List<JourneyDto>>> Handle(GetAllJourneysQuery request, CancellationToken cancellationToken)
        {
            var journeyRepo = unitOfWork.GetRepository<Journey>();
            var query = journeyRepo.Queryable();
            var list = await query.Include(j => j.Hotel)
                .Include(j => j.Company)
                .ToListAsync(cancellationToken);

            var dtoList = list.Select(j => new JourneyDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                StartDate = j.StartDate,
                EndDate = j.EndDate,
                Price = j.Price,
                AvailableSeats = j.AvailableSeats,
                RequiresHotel = j.RequiresHotel
            }).ToList();

            return ApiResponses<List<JourneyDto>>.Success(dtoList);
        }
    }
}
