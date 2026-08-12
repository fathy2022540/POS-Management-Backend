using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.LookupServices.Commands
{
    public record UpdateLookupCommand(long id, string Name, string NameAR) : IRequest<ApiResponses<bool>>;

    public class UpdateLookupCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<UpdateLookupCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateLookupCommand request, CancellationToken cancellationToken)
        {
            var lookupRepo = unitOfWork.GetRepository<Lookup>();


            var existingLookup = await lookupRepo.Find(request.id);
            if (existingLookup == null)
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Lookup not found.");

            AppMapper.Mapper.Map(request, existingLookup);

            await lookupRepo.Update(existingLookup);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Lookup updated successfully.");

        }
    }
}
