using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Application.Common.DTOs;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure;
using POS.Infrastructure.UnitOfWork;

namespace POS.Application.Features.ParamterConfigs.Commands
{
    public record DeleteParamterConfigCommand(long id) : IRequest<ApiResponses<bool>>;

    public class DeleteParamterConfigCommandHandler(IUnitOfWork<POSDBContext> unitOfWork) : IRequestHandler<DeleteParamterConfigCommand, ApiResponses<bool>>
    {
        public async Task<ApiResponses<bool>> Handle(DeleteParamterConfigCommand request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<ParamteresConfiguration>();

            var paramteresConfiguration = await repository.GetFirstOrDefault(
                selector: x => x, 
                predicate: x => x.Id == request.id,
                orderBy: null,
                include: null,
                disableTracking: false
                );

            if (paramteresConfiguration == null)
            {
                return ApiResponses<bool>.Failure(StatusResult.NotFound, "لم يتم العثور على العنصر");
            }

            paramteresConfiguration.Is_Deleted= true;

            await unitOfWork.DoWork();

            return ApiResponses<bool>.Success(true, "تم حذف العنصر بنجاح");


        }
    }



}
