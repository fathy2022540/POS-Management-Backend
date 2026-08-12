using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Features.PermissionServices.Commands;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace Pos.Application.Features.Products.Commands
{
    public record DeleteProductCommand(long Id) : IRequest<ApiResponses<bool>>;
    public class DeleteProductCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<DeleteProductCommand, ApiResponses<bool>>
    {

        public async Task<ApiResponses<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var productRepo = unitOfWork.GetRepository<Product>();
            var product = await productRepo.Find(request.Id);
            if (product == null) return ApiResponses<bool>.Failure(StatusResult.NotFound, "Product not found.");

            await productRepo.Delete(product);
            await unitOfWork.DoWork();
            return ApiResponses<bool>.Success(true, "Product deleted successfully.");
        }
    }
}