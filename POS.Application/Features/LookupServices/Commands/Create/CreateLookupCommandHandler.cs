using MediatR;
using JRM.Application.Common.DTOs;
using JRM.Application.Common.Helper;
using JRM.Application.Helper.Mapper;
using JRM.Domain.Entities;
using JRM.Domain.Enums;
using JRM.Infrastructure;
using JRM.Infrastructure.UnitOfWork;

namespace JRM.Application.Features.LookupServices.Commands
{
    public record CreateLookupCommand(string Name, string NameAR) : IRequest<ApiResponses<bool>>;

    public class CreateLookupCommandHandler(IUnitOfWork<JRMDBContext> unitOfWork) : IRequestHandler<CreateLookupCommand, ApiResponses<bool>>
    {

        public async Task<ApiResponses<bool>> Handle(CreateLookupCommand request, CancellationToken cancellationToken)
        {
            var lookupRepo = unitOfWork.GetRepository<Lookup>();

            // Send all conditions into a single predicate expression using OR (||)
            var conflictingVendor = await BusinessValidator.FindConflictAsync(
                lookupRepo,
                v => v.Name == request.Name
            );

            if (conflictingVendor != null)
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "A lookup with the same name already exists.");


            var newlookup = AppMapper.Mapper.Map<Lookup>(request);
            await lookupRepo.Insert(newlookup);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Lookup created successfully");
        }
    }
}