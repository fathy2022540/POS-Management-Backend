using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.Stores.Commands
{
    public record UpdateStoreCommand(long id, string Name, string Location) : IRequest<ApiResponses<bool>>;
    public class UpdateStoreCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<UpdateStoreCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
        {
            var storeRepo = unitOfWork.GetRepository<Store>();
            var storeData = await storeRepo.GetFirstOrDefault<Store>(null, x => x.Id == request.id, null, null, true);

            if (storeData == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "Store item not found.");
            }

            AppMapper.Mapper.Map(request, storeData);

            await storeRepo.Update(storeData);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Store item updated successfully.");

        }
    }
}