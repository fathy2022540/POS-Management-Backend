using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using MediatR;

namespace JRM.Application.Features.JourneyServices.Commands
{
    public record UpdateJourneyCommand(update Dto) : IRequest<ApiResponses<bool>>;
    public class UpdateJourneyCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
    : IRequestHandler<UpdateJourneyCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateJourneyCommand request, CancellationToken cancellationToken)
        {
            var journeyRepository = unitOfWork.GetRepository<Journey>();
            var journey = await journeyRepository.GetFirstOrDefault<Journey>(null, j => j.Id == request.Id && !j.Is_Deleted, null, null, false);

            if (journey == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "The journey does not exist or has been deleted.");
            }
            AppMapper.Mapper.Map(request, journey);
            await journeyRepository.Update(journey);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Journey updated successfully.");
        }
    }
}
