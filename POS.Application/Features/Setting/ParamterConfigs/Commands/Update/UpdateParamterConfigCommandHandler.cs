using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Application.Common.DTOs;
using POS.Application.Helper.Mapper;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.ParamterConfigs.Commands
{
    public record UpdateParamterConfigCommand(long Id, string Keyword,
        string? Parent,
        string? DescriptionEn,
        string? DescriptionAr,
        string? ContentEn,
        string? ContentAr, string? URL) : IRequest<ApiResponses<bool>>;
    public class UpdateParamterConfigCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<UpdateParamterConfigCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(UpdateParamterConfigCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<Domain.Entities.ParamteresConfiguration>();

            var paramteresConfiguration = await repository.GetFirstOrDefault(
                           selector: x => x,
                           predicate: x => x.Id == request.Id,
                           orderBy: null,
                           include: null,
                           disableTracking: false
                           );

            if (paramteresConfiguration == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "لم يتم العثور على الإعداد المطلوب");
            }

            AppMapper.Mapper.Map(request, paramteresConfiguration);

            await unitOfWork.DoWork();
            return ApiResponses<bool>.Success(true, "تم تحديث الإعداد بنجاح");
        }
    }
}
