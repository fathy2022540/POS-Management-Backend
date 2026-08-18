using MediatR;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.ParamterConfigs.Queries
{
    public record GetParamterConfigByIdQuery(long Id) : IRequest<ApiResponses<paramterConfigRespone>>;

    public class GetParamterConfigByIdQueryHandler(IUnitOfWork<POSDBContext> unitOfWork)
    : IRequestHandler<GetParamterConfigByIdQuery, ApiResponses<paramterConfigRespone>>
    {
        public async Task<ApiResponses<paramterConfigRespone>> Handle(GetParamterConfigByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = unitOfWork.GetRepository<Domain.Entities.ParamteresConfiguration>();

            var entity = await repo.GetFirstOrDefault<Domain.Entities.ParamteresConfiguration>(
                selector: null,
                predicate: x => x.Id == request.Id,
                orderBy: null,
                include: null,
                disableTracking: true
            );

            if (entity == null)
            {
                return ApiResponses<paramterConfigRespone>.Failure(StatusResult.NotFound, "لم يتم العثور على العنصر");
            }

            var resultData = AppMapper.Mapper.Map<paramterConfigRespone>(entity);

            return ApiResponses<paramterConfigRespone>.Success(resultData);
        }
    }
}
