using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using MediatR;

namespace JRM.Application.Features.ReservationServices.Commands.Delete
{
    public record DeleteReservationCommand(long Id) : IRequest<ApiResponses<bool>>;
    public class DeleteReservationCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<DeleteReservationCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
        {
            var reservationRepo = unitOfWork.GetRepository<Reservation>();
            var reservation = await reservationRepo.GetFirstOrDefault<Reservation>(null, r => r.Id == request.Id, null, null, false);

            if (reservation == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Reservation not found.");
            }

            reservation.Is_Deleted = true;

            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Journey deleted successfully.");
        }
    }



}
