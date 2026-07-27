using FluentValidation;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using MediatR;

namespace JRM.Application.Features.ReservationServices.Commands

{
    public sealed record CreateReservationCommand(CreateReservationDto Dto) : IRequest<ApiResponses<bool>>;

    public class CreateReservationCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork, IValidator<CreateReservationDto> _validator) : IRequestHandler<CreateReservationCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            var reservationRepository = unitOfWork.GetRepository<Reservation>();
            var reservationSeatRepository = unitOfWork.GetRepository<ReservationSeat>();
            var hotelReservationRepository = unitOfWork.GetRepository<HotelReservation>();
            var dto = request.Dto ?? throw new ArgumentNullException(nameof(request.Dto));

            var validation = await _validator.ValidateAsync(dto, cancellationToken);
            //if (!validation.IsValid)
            //{
            //    return ApiResponses<bool>.Failure(validation.Errors);
            //}

            // Map DTO to entity
            var reservation = AppMapper.Mapper.Map<Reservation>(dto);

            await reservationRepository.Insert(reservation);
            await unitOfWork.DoWork();
            // link seats if provided
            if (dto.SeatIds != null && dto.SeatIds.Count > 0)
            {
                foreach (var seatId in dto.SeatIds)
                {
                    var rs = new ReservationSeat
                    {
                        ReservationId = reservation.Id,
                        SeatId = seatId
                    };
                    await reservationSeatRepository.Insert(rs);
                }

                await unitOfWork.DoWork();
            }

            // hotel reservations if provided
            if (dto.HotelReservations != null && dto.HotelReservations.Count > 0)
            {
                foreach (var hr in dto.HotelReservations)
                {
                    var hotelRes = new HotelReservation
                    {
                        HotelId = hr.HotelId,
                        ReservationId = reservation.Id,
                        RoomType = hr.RoomType,
                        CheckInDate = hr.CheckInDate,
                        CheckOutDate = hr.CheckOutDate,
                        RoomPriceTotal = hr.RoomPriceTotal
                    };
                    await hotelReservationRepository.Insert(hotelRes);
                }

                await unitOfWork.DoWork();
            }

            //// If completed, send notifications
            //if (reservation.Status == ReservationStatus.Completed)
            //{
            //    var subject = "Reservation Confirmed";
            //    var body = $"Hello {reservation.ClientName},\nYour reservation (Id: {reservation.Id}) for journey {reservation.JourneyId} is confirmed.";
            //    _ = Task.Run(() => _emailSender.SendAsync(reservation.ClientEmail, subject, body, cancellationToken), cancellationToken);
            //    _ = Task.Run(() => _whatsAppSender.SendAsync(reservation.ClientEmail, body, cancellationToken), cancellationToken);
            //}

            return ApiResponses<bool>.Success(true);
        }
    }

}
