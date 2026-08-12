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
    public record CreateLookupItemsCommand(long LookupId, long? LookupItemParentId, string NameEn, string NameAR, string? Code) : IRequest<ApiResponses<bool>>;

    public class CreateLookupItemsCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<CreateLookupItemsCommand, ApiResponses<bool>>
    {

        public async Task<ApiResponses<bool>> Handle(CreateLookupItemsCommand request, CancellationToken cancellationToken)
        {
            var lookupItemsRepo = unitOfWork.GetRepository<LookupItems>();

            // Send all conditions into a single predicate expression using OR (||)
            var conflictingVendor = await BusinessValidator.FindConflictAsync(
                lookupItemsRepo,
                v => v.NameEn == request.NameEn || v.NameAR == request.NameAR
            );

            if (conflictingVendor != null)
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "A lookup with the same name already exists.");


            var newlookupItems = AppMapper.Mapper.Map<LookupItems>(request);
            await lookupItemsRepo.Insert(newlookupItems);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Lookup item created successfully");
        }
    }
}