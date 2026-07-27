using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.LookupServices.Commands
{
    public record UpdateLookupCommand(long id, string Name, string NameAR) : IRequest<ApiResponses<bool>>;

    public class UpdateLookupCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork) : IRequestHandler<UpdateLookupCommand, ApiResponses<bool>>
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
