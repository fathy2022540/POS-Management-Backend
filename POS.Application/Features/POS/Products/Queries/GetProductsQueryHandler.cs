using MediatR;
using Microsoft.EntityFrameworkCore;
using POS.Application.Common.DTOs;
using POS.Application.Features.PermissionServices.Queries;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace Pos.Application.Features.Products.Queries
{
    public class GetAllProductQuery : Pagination, IRequest<ApiResponses<PaginationResponse<ProductDto>>>
    {
    }
    public class GetProductsQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
        : IRequestHandler<GetAllProductQuery, ApiResponses<PaginationResponse<ProductDto>>>
    {

        public async Task<ApiResponses<PaginationResponse<ProductDto>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            var ProductitemsRepo = unitOfWork.GetRepository<Product>();

            var products = await ProductitemsRepo.GetList(
                predicate: p => !p.Is_Deleted && p.IsActive,
                orderBy: q => q.OrderByDescending(p => p.CreatedDate),
                include: null,
                disableTracking: true,
                skip: (request.PageNumber - 1) * request.PageSize,
                take: request.PageSize);

            if (!products.Any())
            {
                return ApiResponses<PaginationResponse<ProductDto>>.Failure(StatusResult.NotFound, "No product items found.");
            }

            var paginationResponse = new PaginationResponse<ProductDto>
            {
                Data = AppMapper.Mapper.Map<IEnumerable<ProductDto>>(products),
                Count = await ProductitemsRepo.CountAsync(p => !p.Is_Deleted && p.IsActive),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return ApiResponses<PaginationResponse<ProductDto>>.Success(paginationResponse, "Products retrieved successfully.");
        }
    }
}