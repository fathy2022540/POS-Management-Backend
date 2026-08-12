using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.LookupServices.Commands
{
    public record CreateLookupCommand(string Name, string NameAR) : IRequest<ApiResponses<bool>>;

    public class CreateLookupCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<CreateLookupCommand, ApiResponses<bool>>
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