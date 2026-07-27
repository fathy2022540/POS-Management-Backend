using JRM.Application.Common.DTOs;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;
using MediatR;

namespace JRM.Application.Features.ProductServices.Commands
{
    public record DeleteJourneyCommand(long Id) : IRequest<ApiResponses<bool>>;
    public class DeleteJourneyCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork)
        : IRequestHandler<DeleteJourneyCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(DeleteJourneyCommand request, CancellationToken cancellationToken)
        {
            var journeyRepo = unitOfWork.GetRepository<Journey>();

            var journey = await journeyRepo.GetFirstOrDefault<Journey>(null, j => j.Id == request.Id, null, null, false);

            if (journey == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Journey not found.");
            }

            journey.Is_Deleted = true;

            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Journey deleted successfully.");
        }
    }
}
