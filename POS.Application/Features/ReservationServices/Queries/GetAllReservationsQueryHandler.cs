using JRM.Application.Common.DTOs;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JRM.Application.Features.ReservationServices.Queries
{
    public sealed record GetAllReservationsQuery() : IRequest<ApiResponses<List<ReservationDto>>>;

    public class GetAllReservationsQueryHandler : IRequestHandler<GetAllReservationsQuery, ApiResponses<List<ReservationDto>>>
    {
        private readonly IUnitOfWork<JRMDBContext> _unitOfWork;

        public GetAllReservationsQueryHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponses<List<ReservationDto>>> Handle(GetAllReservationsQuery request, CancellationToken cancellationToken)
        {
            var repo = _unitOfWork.Repository<Domain.Entities.Reservation>();
            var query = repo.Queryable(); // assume repository exposes IQueryable
            var list = await query
                .Include(r => r.ReservedSeats)
                .Include(r => r.HotelReservations)
                .ToListAsync(cancellationToken);

            var dtoList = list.Select(r => new ReservationDto
            {
                Id = r.Id,
                JourneyId = r.JourneyId,
                ClientName = r.ClientName,
                ClientEmail = r.ClientEmail,
                BookingDate = r.BookingDate,
                NumberOfSeats = r.NumberOfSeats,
                TotalPrice = r.TotalPrice,
                Status = r.Status.ToString()
            }).ToList();

            return ApiResponses<List<ReservationDto>>.Success(dtoList);
        }
    }
}
