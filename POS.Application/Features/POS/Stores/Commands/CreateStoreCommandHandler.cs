using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Stores.Commands
{
    public record CreateStoreCommand(string? Name, string Location) : IRequest<ApiResponses<bool>>;

    public class CreateStoreCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<CreateStoreCommand, ApiResponses<bool>>
    {


        public async Task<ApiResponses<bool>> Handle(CreateStoreCommand request, CancellationToken cancellationToken)
        {
            var storesRepo = unitOfWork.GetRepository<Store>();        // Optional: Check if store name already exists to prevent duplicates
            var storeExists = await BusinessValidator.FindConflictAsync(storesRepo, v => v.Name.ToLower() == request.Name.ToLower());
            if (storeExists != null)
            {
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "Store name already exists.");
            }
            var newStore = AppMapper.Mapper.Map<Store>(request);
            await storesRepo.Insert(newStore);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Store created successfully.");
        }
    }
}