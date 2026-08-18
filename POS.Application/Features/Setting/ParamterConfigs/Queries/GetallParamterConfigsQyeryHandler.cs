using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.ParamterConfigs.Queries
{

    public class GetallParamterConfigsQyery : Pagination, IRequest<ApiResponses<PaginationResponse<ParamterConfigListRespone>>>
    {

    }

    public class GetallParamterConfigsQyeryHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<GetallParamterConfigsQyery, ApiResponses<PaginationResponse<ParamterConfigListRespone>>>
    {
        public async Task<ApiResponses<PaginationResponse<ParamterConfigListRespone>>> Handle(GetallParamterConfigsQyery request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<ParamteresConfiguration>();

            int skip = (request.PageNumber - 1) * request.PageSize;

            var paramterConfigs = await repository.GetList(
                predicate: p => !p.Is_Deleted,
                orderBy: q => q.OrderByDescending(p => p.CreatedDate),
                include: null,
                disableTracking: true,
                skip: skip,
                take: request.PageSize
            );

            if (paramterConfigs == null || !paramterConfigs.Any())
            {
                return ApiResponses<PaginationResponse<ParamterConfigListRespone>>.Failure(
                    StatusResult.NotFound,
                    "لا توجد بيانات متاحة لعرضها حالياً"
                );
            }

            var resultData = AppMapper.Mapper.Map<List<ParamterConfigListRespone>>(paramterConfigs);

            var paginationResult = new PaginationResponse<ParamterConfigListRespone>
            {
                Count = resultData.Count,
                Data = resultData,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            return ApiResponses<PaginationResponse<ParamterConfigListRespone>>.Success(paginationResult, "تم جلب البيانات بنجاح");


        }
    }
}
