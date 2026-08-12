using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Common.Helper;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;


namespace POS.Application.Features.Products.Commands
{
    public record CreateProductCommand(CreateProductDto Product) : IRequest<ApiResponses<bool>>;
    public class CreateProductCommandHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<CreateProductCommand, ApiResponses<bool>>
    {


        public async Task<ApiResponses<bool>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productRepo = unitOfWork.GetRepository<Product>();

            // Check if Code already exists (Codes should be unique identifiers like "USER_CREATE")
            var codeExists = await BusinessValidator.FindConflictAsync(productRepo, v => v.Code.ToLower() == request.Product.Code.ToLower() || v.Name.ToLower() == request.Product.Name.ToLower());
            if (codeExists != null)
            {
                return ApiResponses<bool>.Failure(StatusResult.DataAlreadyExist, "Product code already exists.");
            }

            var newProduct = AppMapper.Mapper.Map<Product>(request.Product);

            await productRepo.Insert(newProduct);
            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "Product created successfully.");
        }
    }
}