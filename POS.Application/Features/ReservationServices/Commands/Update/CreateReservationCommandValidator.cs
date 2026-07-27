using JRM.Application.Common.DTOs;
using JRM.Application.Common.Helper;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using MediatR;

namespace JRM.Application.Features.JourneyServices.Commands
{

    public record CreateJourneyCommand(string Title, string Description, JourneyType Type, DateTime StartDate, DateTime EndDate, decimal Price, int AvailableSeats, long CompanyId, long? BusId, bool RequiresHotel, long? HotelId) : IRequest<ApiResponses<bool>>;
    public class CreateJourneyCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork) : IRequestHandler<CreateJourneyCommand, ApiResponses<bool>>
    {

        public async Task<ApiResponses<bool>> Handle(CreateJourneyCommand request, CancellationToken cancellationToken)
        {

            var journeyRepository = unitOfWork.GetRepository<Journey>();
            var conflictingJourney = await BusinessValidator.FindConflictAsync(journeyRepository, v =>
            v.Title == request.Title || v.StartDate == request.StartDate);
            if (conflictingJourney != null)
            {
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "A journey with this title or start date already exists.");
            }
            var journey = AppMapper.Mapper.Map<Journey>(request);
            //journey.ItemCode = request.ProductCode;
            journey.IsActive = true;

            await journeyRepository.Insert(journey);
            await unitOfWork.DoWork();
            return ApiResponses<bool>.Success(true, "Journey created successfully.");
        }
    }
}
