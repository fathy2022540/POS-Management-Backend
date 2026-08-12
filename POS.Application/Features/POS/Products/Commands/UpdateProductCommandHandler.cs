using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace Pos.Application.Features.Products.Commands
{
    public record UpdateProductCommand(UpdateProductDto product) : IRequest<ApiResponses<bool>>;
    public class UpdateProductCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<UpdateProductCommand, ApiResponses<bool>>
    {

        public async Task<ApiResponses<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productRepo = unitOfWork.GetRepository<Product>();
            var product = await productRepo.Find(request.product.Id);
            if (product == null) return ApiResponses<bool>.Failure(StatusResult.NotFound, "Product not found.");

            AppMapper.Mapper.Map(request, product);

            await productRepo.Update(product);
            await unitOfWork.DoWork();
            return ApiResponses<bool>.Success(true, "Product updated successfully.");
        }
    }
}